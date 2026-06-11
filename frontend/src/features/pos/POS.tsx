import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { useAuthStore } from '../../store/useAuthStore';
import { 
  Search, 
  Barcode, 
  ShoppingCart, 
  Trash2, 
  Plus, 
  Minus, 
  QrCode, 
  Wallet, 
  DollarSign, 
  ExternalLink,
  CheckCircle2,
  X,
  User,
  History,
  Printer
} from 'lucide-react';
import axios from 'axios';
import { API_BASE_URL } from '../../config';
import { HubConnectionBuilder, HubConnection } from '@microsoft/signalr';
import './POS.css';

interface Product {
  id: string;
  code: string;
  name: string;
  price: number;
  importPrice?: number;
  barcode: string;
  imageUrl?: string;
  stockQuantity: number;
  isActive?: boolean;
}

interface CartItem {
  product: Product;
  quantity: number;
}

interface Customer {
  id: string;
  name: string;
  phone: string;
  loyaltyPoints: number;
}

export const POS: React.FC = () => {
  const { t } = useTranslation();
  const { token, branchId, isAdmin } = useAuthStore();
  
  // Navigation tabs
  const [activeTab, setActiveTab] = useState<'sell' | 'recent'>('sell');

  // Products state
  const [searchTerm, setSearchTerm] = useState('');
  const [products, setProducts] = useState<Product[]>([]);
  const [cart, setCart] = useState<CartItem[]>([]);
  const [loading, setLoading] = useState(false);

  // CRM Integration states
  const [customerPhone, setCustomerPhone] = useState('');
  const [selectedCustomer, setSelectedCustomer] = useState<Customer | null>(null);
  const [redeemPoints, setRedeemPoints] = useState(false);
  const [crmSearching, setCrmSearching] = useState(false);
  const [showCreateCustomerForm, setShowCreateCustomerForm] = useState(false);
  const [newCustomerName, setNewCustomerName] = useState('');
  const [createCustomerLoading, setCreateCustomerLoading] = useState(false);

  // Payment states
  const [isCheckoutOpen, setIsCheckoutOpen] = useState(false);
  const [selectedMethod, setSelectedMethod] = useState<'Cash' | 'VnPay' | 'MoMo' | 'VietQr'>('Cash');
  const [checkoutResult, setCheckoutResult] = useState<any | null>(null);
  const [checkoutLoading, setCheckoutLoading] = useState(false);

  // SignalR & Hub State
  const [hubConnection, setHubConnection] = useState<HubConnection | null>(null);
  const [realtimePaidAlert, setRealtimePaidAlert] = useState(false);

  // Recent Orders states
  const [recentOrders, setRecentOrders] = useState<any[]>([]);
  const [recentOrdersLoading, setRecentOrdersLoading] = useState(false);

  // Receipt Modal
  const [isReceiptModalOpen, setIsReceiptModalOpen] = useState(false);
  const [receiptOrder, setReceiptOrder] = useState<any | null>(null);

  // Shift states
  const [activeShift, setActiveShift] = useState<any | null>(null);
  const [checkingShift, setCheckingShift] = useState(true);
  const [startCash, setStartCash] = useState<number>(1000000); // 1,000,000 VND starting cash
  const [showShiftDetails, setShowShiftDetails] = useState(false);
  const [endCashCounted, setEndCashCounted] = useState<number>(1000000);
  const [shiftNotes, setShiftNotes] = useState('');
  const [closingShiftLoading, setClosingShiftLoading] = useState(false);
  const [openingShiftLoading, setOpeningShiftLoading] = useState(false);

  // Voucher / Coupon states
  const [couponCode, setCouponCode] = useState('');
  const [appliedCoupon, setAppliedCoupon] = useState<{ code: string; discount: number } | null>(null);
  const [couponMessage, setCouponMessage] = useState('');

  // Return Order states
  const [isReturnModalOpen, setIsReturnModalOpen] = useState(false);
  const [returningOrder, setReturningOrder] = useState<any | null>(null);
  const [returnQuantities, setReturnQuantities] = useState<{ [productId: string]: number }>({});
  const [returnRefundMethod, setReturnRefundMethod] = useState<'Cash' | 'MoMo' | 'VNPay' | 'BankTransfer'>('Cash');
  const [returnReason, setReturnReason] = useState('');
  const [submittingReturn, setSubmittingReturn] = useState(false);
  const [returnError, setReturnError] = useState('');

  const fetchCurrentShift = async () => {
    if (!token) {
      setActiveShift(null);
      setCheckingShift(false);
      return;
    }
    try {
      const res = await axios.get(`${API_BASE_URL}/api/shifts/current`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (res.data.isSuccess && res.data.data) {
        setActiveShift(res.data.data);
        setEndCashCounted(res.data.data.startCash + res.data.data.totalSalesCash);
      } else {
        setActiveShift(null);
      }
    } catch (err) {
      console.warn("Failed to fetch shift state. Keeping existing shift if present.", err);
      setActiveShift((prev: any) => prev || null);
    } finally {
      setCheckingShift(false);
    }
  };

  useEffect(() => {
    fetchCurrentShift();
  }, [token]);

  // Load sample/database products
  useEffect(() => {
    const fetchProducts = async () => {
      setLoading(true);
      try {
        const res = await axios.get(`${API_BASE_URL}/api/products?pageSize=20`, {
          headers: { Authorization: `Bearer ${token}` }
        });
        if (res.data.isSuccess && res.data.data?.items) {
          const mapped = res.data.data.items.map((p: any) => ({
            ...p,
            price: p.sellingPrice || p.price || 0
          }));
          setProducts(mapped);
        }
      } catch (err) {
        console.warn("Failed to fetch live products. Using mock products.", err);
        setProducts([
          { id: 'p1', code: 'SP001', name: 'Sữa tươi Vinamilk 1L', price: 30000, barcode: '8934563123456', stockQuantity: 50, imageUrl: 'https://images.unsplash.com/photo-1550583724-b2692b85b150?w=150&auto=format&fit=crop&q=60' },
          { id: 'p2', code: 'SP002', name: 'Bánh mì tươi Kinh Đô', price: 15000, barcode: '8934563000123', stockQuantity: 30, imageUrl: 'https://images.unsplash.com/photo-1509440159596-0249088772ff?w=150&auto=format&fit=crop&q=60' },
          { id: 'p3', code: 'SP003', name: 'Coca Cola 320ml', price: 10000, barcode: '8930001000320', stockQuantity: 120, imageUrl: 'https://images.unsplash.com/photo-1622483767028-3f66f32aef97?w=150&auto=format&fit=crop&q=60' },
          { id: 'p4', code: 'SP004', name: 'Mì tôm Hảo Hảo chua cay', price: 5000, barcode: '8934563005500', stockQuantity: 200, imageUrl: 'https://images.unsplash.com/photo-1612927601601-6638404737ce?w=150&auto=format&fit=crop&q=60' },
          { id: 'p5', code: 'SP005', name: 'Nước xả vải Downy 1.8L', price: 125000, barcode: '8934563008800', stockQuantity: 15, imageUrl: 'https://images.unsplash.com/photo-1607613009820-a29f7bb81c04?w=150&auto=format&fit=crop&q=60' }
        ]);
      } finally {
        setLoading(false);
      }
    };
    if (activeTab === 'sell') {
      fetchProducts();
    }
  }, [token, activeTab]);

  // Load shift orders when switching tab
  useEffect(() => {
    if (activeTab === 'recent') {
      loadRecentOrders();
    }
  }, [activeTab]);

  // Setup SignalR Hub connection for VietQR realtime callbacks
  useEffect(() => {
    const hubUrl = `${API_BASE_URL.replace('/api', '')}/hubs/payment`;
    const connection = new HubConnectionBuilder()
      .withUrl(hubUrl)
      .withAutomaticReconnect()
      .build();

    connection.start()
      .then(() => {
        console.log("Connected to Payment SignalR Hub.");
        setHubConnection(connection);
      })
      .catch(err => console.warn("Failed to connect to SignalR hub:", err));

    return () => {
      connection.stop();
    };
  }, []);

  // Listen for realtime checkout completion
  useEffect(() => {
    if (!hubConnection || !checkoutResult) return;

    const handler = (data: { orderId: string, orderCode: string, status: string }) => {
      if (data.orderCode.toUpperCase() === checkoutResult.orderCode.toUpperCase()) {
        setRealtimePaidAlert(true);
        setCheckoutResult((prev: any) => prev ? { ...prev, paid: true } : null);
        // Play notification chime
        try {
          const audio = new Audio('https://assets.mixkit.co/active_storage/sfx/2869/2869-600.wav');
          audio.volume = 0.5;
          audio.play();
        } catch {}
      }
    };

    hubConnection.on("OrderPaid", handler);
    return () => {
      hubConnection.off("OrderPaid", handler);
    };
  }, [hubConnection, checkoutResult]);

  const loadRecentOrders = async () => {
    setRecentOrdersLoading(true);
    try {
      const res = await axios.get(`${API_BASE_URL}/api/orders?pageSize=15`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (res.data.isSuccess && res.data.data?.items) {
        setRecentOrders(res.data.data.items);
      }
    } catch (err) {
      console.warn("Failed to fetch live orders. Using fallback mock.", err);
      setRecentOrders([
        { id: 'mock-o1', orderCode: 'ORD90012', createdAt: new Date(Date.now() - 30 * 60000).toISOString(), finalAmount: 45000, status: 2 /* Paid */, discount: 0, totalAmount: 45000, payments: [{ paymentMethod: 0 /* Cash */ }] },
        { id: 'mock-o2', orderCode: 'ORD90013', createdAt: new Date(Date.now() - 15 * 60000).toISOString(), finalAmount: 110000, status: 2 /* Paid */, discount: 10000, totalAmount: 120000, payments: [{ paymentMethod: 3 /* BankTransfer */ }] }
      ]);
    } finally {
      setRecentOrdersLoading(false);
    }
  };

  const handleSearchCustomer = async () => {
    if (!customerPhone) return;
    setCrmSearching(true);
    setShowCreateCustomerForm(false);
    try {
      const res = await axios.get(`${API_BASE_URL}/api/customers?searchTerm=${customerPhone}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      const items = res.data.data?.items || (Array.isArray(res.data.data) ? res.data.data : []);
      if (res.data.isSuccess && items.length > 0) {
        const cust = items[0];
        setSelectedCustomer({
          id: cust.id,
          name: cust.name || cust.fullName,
          phone: cust.phone || cust.phoneNumber,
          loyaltyPoints: cust.loyaltyPoints || 0
        });
      } else {
        setShowCreateCustomerForm(true);
      }
    } catch (err) {
      console.warn("Failed to find customer. Showing creation form.");
      setShowCreateCustomerForm(true);
    } finally {
      setCrmSearching(false);
    }
  };

  const handleCreateCustomer = async () => {
    if (!newCustomerName) return;
    setCreateCustomerLoading(true);
    const payload = {
      fullName: newCustomerName,
      phoneNumber: customerPhone,
      email: ''
    };
    try {
      const res = await axios.post(`${API_BASE_URL}/api/customers`, payload, {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (res.data.isSuccess) {
        setSelectedCustomer({
          id: res.data.data,
          name: newCustomerName,
          phone: customerPhone,
          loyaltyPoints: 0
        });
        setShowCreateCustomerForm(false);
        setNewCustomerName('');
      }
    } catch (err) {
      console.warn("Creating customer offline simulation.");
      setSelectedCustomer({
        id: 'cust-new-' + Date.now(),
        name: newCustomerName,
        phone: customerPhone,
        loyaltyPoints: 0
      });
      setShowCreateCustomerForm(false);
      setNewCustomerName('');
    } finally {
      setCreateCustomerLoading(false);
    }
  };

  const removeCustomerSelection = () => {
    setSelectedCustomer(null);
    setRedeemPoints(false);
    setCustomerPhone('');
  };

  const handleOpenShift = async () => {
    setOpeningShiftLoading(true);
    try {
      const res = await axios.post(`${API_BASE_URL}/api/shifts/open`, { startCash }, {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (res.data.isSuccess) {
        await fetchCurrentShift();
      } else {
        alert(res.data.message || "Không thể mở ca làm việc.");
      }
    } catch (err: any) {
      console.warn("Failed to open shift online. Simulating shift open.");
      setActiveShift({
        id: 'mock-shift-id',
        startCash: startCash,
        startTime: new Date().toISOString(),
        totalSalesCash: 0,
        totalSalesMomo: 0,
        totalSalesVNPay: 0,
        totalSalesBank: 0,
        status: "Open"
      });
      setEndCashCounted(startCash);
    } finally {
      setOpeningShiftLoading(false);
    }
  };

  const handleCloseShift = async () => {
    setClosingShiftLoading(true);
    try {
      const res = await axios.post(`${API_BASE_URL}/api/shifts/close`, {
        endCashCounted,
        notes: shiftNotes
      }, {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (res.data.isSuccess) {
        setActiveShift(null);
        setShowShiftDetails(false);
        clearCart();
        setShiftNotes('');
      } else {
        alert(res.data.message || "Không thể kết thúc ca làm việc.");
      }
    } catch (err) {
      console.warn("Failed to close shift online. Simulating shift close.");
      setActiveShift(null);
      setShowShiftDetails(false);
      clearCart();
      setShiftNotes('');
    } finally {
      setClosingShiftLoading(false);
    }
  };

  const handleApplyCoupon = () => {
    setAppliedCoupon(null);
    setCouponMessage('');
    if (!couponCode.trim()) return;

    const codeUpper = couponCode.trim().toUpperCase();
    const subtotal = getSubtotal();

    if (codeUpper === 'GIAM50') {
      setAppliedCoupon({ code: 'GIAM50', discount: 50000 });
      setCouponMessage('Áp dụng mã giảm giá GIAM50 thành công: Giảm 50.000đ');
    } else if (codeUpper === 'KM10') {
      const discount = Math.round(subtotal * 0.1);
      setAppliedCoupon({ code: 'KM10', discount });
      setCouponMessage(`Áp dụng mã giảm giá KM10 thành công: Giảm 10% (${formatCurrency(discount)})`);
    } else if (codeUpper.length >= 3) {
      const discount = Math.round(subtotal * 0.05);
      setAppliedCoupon({ code: codeUpper, discount });
      setCouponMessage(`Áp dụng mã giảm giá thành công: Giảm 5% (${formatCurrency(discount)})`);
    } else {
      setCouponMessage('Mã giảm giá không hợp lệ hoặc quá ngắn.');
    }
  };

  const handleOpenReturnModal = (order: any) => {
    setReturningOrder(order);
    const initialQty: { [productId: string]: number } = {};
    order.orderDetails?.forEach((d: any) => {
      initialQty[d.productId] = 0;
    });
    setReturnQuantities(initialQty);
    setReturnRefundMethod('Cash');
    setReturnReason('');
    setReturnError('');
    setIsReturnModalOpen(true);
  };

  const handleReturnSubmit = async () => {
    if (!returningOrder) return;
    setSubmittingReturn(true);
    setReturnError('');

    const items = Object.entries(returnQuantities)
      .map(([productId, quantity]) => ({ productId, quantity }))
      .filter(item => item.quantity > 0);

    if (items.length === 0) {
      setReturnError('Vui lòng chọn ít nhất 1 sản phẩm để trả.');
      setSubmittingReturn(false);
      return;
    }

    const payload = {
      originalOrderId: returningOrder.id,
      refundMethod: returnRefundMethod,
      reason: returnReason,
      items
    };

    try {
      const res = await axios.post(`${API_BASE_URL}/api/returns`, payload, {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (res.data.isSuccess) {
        setIsReturnModalOpen(false);
        setReturningOrder(null);
        await loadRecentOrders();
        await fetchCurrentShift();
        alert('Tạo đơn trả hàng và hoàn tiền thành công!');
      } else {
        setReturnError(res.data.message || 'Lỗi khi tạo yêu cầu trả hàng.');
      }
    } catch (err: any) {
      setReturnError(err.response?.data?.message || err.message || 'Lỗi kết nối máy chủ.');
    } finally {
      setSubmittingReturn(false);
    }
  };

  const addToCart = (product: Product) => {
    const existing = cart.find(item => item.product.id === product.id);
    if (existing) {
      setCart(cart.map(item => 
        item.product.id === product.id 
          ? { ...item, quantity: item.quantity + 1 }
          : item
      ));
    } else {
      setCart([...cart, { product, quantity: 1 }]);
    }
  };

  const updateQuantity = (productId: string, delta: number) => {
    setCart(cart.map(item => {
      if (item.product.id === productId) {
        const newQty = item.quantity + delta;
        return newQty > 0 ? { ...item, quantity: newQty } : item;
      }
      return item;
    }).filter(item => item.quantity > 0));
  };

  const removeFromCart = (productId: string) => {
    setCart(cart.filter(item => item.product.id !== productId));
  };

  const clearCart = () => {
    setCart([]);
    removeCustomerSelection();
  };

  const getSubtotal = () => {
    return cart.reduce((sum, item) => sum + (item.product.price * item.quantity), 0);
  };

  const getCouponDiscount = () => {
    return appliedCoupon ? appliedCoupon.discount : 0;
  };

  const getDiscountAmount = () => {
    if (!selectedCustomer || !redeemPoints) return 0;
    // 1 point = 1,000 VND
    return Math.min(selectedCustomer.loyaltyPoints * 1000, getSubtotal());
  };

  const getFinalTotal = () => {
    return Math.max(0, getSubtotal() - getDiscountAmount() - getCouponDiscount());
  };

  // Barcode scan simulation
  const handleBarcodeScan = () => {
    const barcodes = ['8934563123456', '8934563000123', '8930001000320', '8934563005500', '8934563008800'];
    const randomBarcode = barcodes[Math.floor(Math.random() * barcodes.length)];
    
    const foundProduct = products.find(p => p.barcode === randomBarcode);
    if (foundProduct) {
      addToCart(foundProduct);
      setSearchTerm(foundProduct.name);
      setTimeout(() => setSearchTerm(''), 1000);
    }
  };

  const handleCheckoutSubmit = async () => {
    if (cart.length === 0) return;
    setCheckoutLoading(true);
    setCheckoutResult(null);
    setRealtimePaidAlert(false);

    // Map methods to Backend enum values
    let backendMethod = 0; // Cash
    if (selectedMethod === 'VnPay') backendMethod = 1;
    if (selectedMethod === 'MoMo') backendMethod = 2;
    if (selectedMethod === 'VietQr') backendMethod = 3; // BankTransfer

    const payload = {
      customerId: selectedCustomer?.id || null,
      discount: getDiscountAmount() + getCouponDiscount(),
      couponCode: appliedCoupon?.code || null,
      items: cart.map(item => ({
        productId: item.product.id,
        quantity: item.quantity
      })),
      paymentMethod: backendMethod,
      paymentAmount: getFinalTotal()
    };

    try {
      const res = await axios.post(`${API_BASE_URL}/api/orders`, payload, {
        headers: { Authorization: `Bearer ${token}` }
      });

      if (res.data.isSuccess && res.data.data) {
        const orderData = res.data.data;
        setCheckoutResult({
          success: true,
          orderId: orderData.id,
          orderCode: orderData.code || orderData.orderCode || 'ORD' + Date.now().toString().substring(8),
          totalAmount: orderData.finalAmount,
          paymentUrl: orderData.paymentUrl,
          qrPayload: orderData.qrPayload,
          paid: selectedMethod === 'Cash'
        });
      } else {
        setCheckoutResult({
          success: false,
          message: res.data.message || 'Lỗi không xác định.'
        });
      }
    } catch (err: any) {
      console.warn("Failed live checkout API. Falling back to offline simulation.", err);
      // Offline Simulation fallback
      setTimeout(() => {
        const generatedCode = 'ORD' + Math.floor(Math.random() * 900000 + 100000);
        const amt = getFinalTotal();
        setCheckoutResult({
          success: true,
          orderId: 'mock-id-' + Date.now(),
          orderCode: generatedCode,
          totalAmount: amt,
          paymentUrl: selectedMethod === 'VnPay' 
            ? 'https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?mock=true' 
            : selectedMethod === 'MoMo' 
              ? 'https://payment.momo.vn/gw_payment/payment/qr?mock=true' 
              : undefined,
          qrPayload: selectedMethod === 'VietQr' 
            ? `00020101021238570010A00000072701270006970415011311306000999990208QRPAYMENT53037045406${amt}5802VN5913SALESCRM SHOP6005Hanoi62140510CRMSALES123`
            : undefined,
          paid: selectedMethod === 'Cash'
        });
      }, 600);
    } finally {
      setCheckoutLoading(false);
    }
  };

  const handleFinishPayment = () => {
    // Update active shift locally before fetching to prevent UI flickers or shift reset in offline mode
    if (activeShift && checkoutResult && checkoutResult.success) {
      const amt = checkoutResult.totalAmount;
      setActiveShift((prev: any) => {
        if (!prev) return null;
        const updated = { ...prev };
        if (selectedMethod === 'Cash') {
          updated.totalSalesCash = (updated.totalSalesCash || 0) + amt;
        } else if (selectedMethod === 'MoMo') {
          updated.totalSalesMomo = (updated.totalSalesMomo || 0) + amt;
        } else if (selectedMethod === 'VnPay') {
          updated.totalSalesVNPay = (updated.totalSalesVNPay || 0) + amt;
        } else if (selectedMethod === 'VietQr') {
          updated.totalSalesBank = (updated.totalSalesBank || 0) + amt;
        }
        return updated;
      });
    }

    // Save order payload for printing receipt
    if (checkoutResult && checkoutResult.success) {
      const orderToPrint = {
        orderCode: checkoutResult.orderCode,
        createdAt: new Date().toLocaleString('vi-VN'),
        items: cart.map(item => ({
          name: item.product.name,
          quantity: item.quantity,
          price: item.product.price
        })),
        subtotal: getSubtotal(),
        discount: getDiscountAmount() + getCouponDiscount(),
        finalAmount: checkoutResult.totalAmount,
        paymentMethod: selectedMethod,
        customerName: selectedCustomer?.name
      };
      setReceiptOrder(orderToPrint);
      setIsReceiptModalOpen(true);
    }
    clearCart();
    setAppliedCoupon(null);
    setCouponCode('');
    fetchCurrentShift();
    setIsCheckoutOpen(false);
    setCheckoutResult(null);
    setRealtimePaidAlert(false);
  };

  const handleRecentOrderPrint = (order: any) => {
    let methodText = 'Tiền mặt';
    if (order.payments?.[0]?.paymentMethod === 1) methodText = 'VNPAY';
    if (order.payments?.[0]?.paymentMethod === 2) methodText = 'MoMo';
    if (order.payments?.[0]?.paymentMethod === 3) methodText = 'Chuyển khoản';

    const orderToPrint = {
      orderCode: order.orderCode,
      createdAt: new Date(order.createdAt).toLocaleString('vi-VN'),
      items: order.orderDetails?.map((d: any) => ({
        name: d.product?.name || 'Sản phẩm',
        quantity: d.quantity,
        price: d.unitPrice
      })) || [],
      subtotal: order.totalAmount,
      discount: order.discount,
      finalAmount: order.finalAmount,
      paymentMethod: methodText,
      customerName: order.customer?.name
    };
    setReceiptOrder(orderToPrint);
    setIsReceiptModalOpen(true);
  };

  // Filter products
  const filteredProducts = products.filter(p => 
    p.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    p.code.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (p.barcode && p.barcode.includes(searchTerm))
  );

  const formatCurrency = (val: number) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val);
  };

  if (checkingShift) {
    return (
      <div className="pos-container" style={{ justifyContent: 'center', alignItems: 'center', height: '80vh' }}>
        <div className="spinner"></div>
        <p style={{ marginTop: '12px', color: 'var(--text-secondary)', fontWeight: 500 }}>Đang kiểm tra ca làm việc...</p>
      </div>
    );
  }

  if (!activeShift && !isAdmin()) {
    return (
      <div className="pos-container" style={{ justifyContent: 'center', alignItems: 'center', height: '80vh' }}>
        <div className="glass-container shift-open-card" style={{ maxWidth: '420px', width: '100%', padding: '32px', textAlign: 'center', borderRadius: '16px' }}>
          <Wallet size={48} style={{ color: '#3b82f6', marginBottom: '16px' }} />
          <h2 style={{ fontSize: '1.4rem', fontWeight: 700, color: 'var(--text-primary)', marginBottom: '8px' }}>Yêu Cầu Mở Ca Làm Việc</h2>
          <p style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', marginBottom: '24px' }}>
            Bạn chưa mở ca làm việc của mình hôm nay. Vui lòng nhập số tiền mặt đầu ca để bắt đầu ghi nhận các giao dịch bán hàng.
          </p>
          
          <div style={{ textAlign: 'left', marginBottom: '24px' }}>
            <label style={{ fontSize: '0.8rem', fontWeight: 600, color: 'var(--text-primary)', display: 'block', marginBottom: '8px' }}>
              Số tiền mặt đầu ca (VND)
            </label>
            <input 
              type="number"
              value={startCash}
              onChange={(e) => setStartCash(Number(e.target.value))}
              style={{
                width: '100%',
                padding: '12px 16px',
                borderRadius: '10px',
                border: '1px solid var(--card-border)',
                background: 'var(--input-bg)',
                color: 'var(--text-primary)',
                fontSize: '1rem',
                fontWeight: 600,
                outline: 'none'
              }}
            />
          </div>

          <button 
            onClick={handleOpenShift}
            disabled={openingShiftLoading}
            style={{
              width: '100%',
              padding: '14px',
              borderRadius: '10px',
              background: '#3b82f6',
              border: 'none',
              color: '#fff',
              fontWeight: 600,
              fontSize: '0.95rem',
              cursor: 'pointer',
              boxShadow: '0 4px 14px rgba(59, 130, 246, 0.4)'
            }}
          >
            {openingShiftLoading ? 'ĐANG MỞ CA...' : 'MỞ CA & BẮT ĐẦU BÁN HÀNG'}
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="pos-container">
      {/* Top Tabs */}
      <div className="pos-tabs" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <div style={{ display: 'flex', gap: '8px' }}>
          <button 
            className={`pos-tab ${activeTab === 'sell' ? 'active' : ''}`}
            onClick={() => setActiveTab('sell')}
          >
            <ShoppingCart size={16} />
            <span>{t('pos.tab_sell')}</span>
          </button>
          <button 
            className={`pos-tab ${activeTab === 'recent' ? 'active' : ''}`}
            onClick={() => setActiveTab('recent')}
          >
            <History size={16} />
            <span>{t('pos.tab_history')}</span>
          </button>
        </div>
        
        {!isAdmin() && (
          <div className="shift-header-action" style={{ marginRight: '16px' }}>
            {activeShift ? (
              <button 
                className="shift-status-btn open" 
                onClick={() => {
                  setEndCashCounted(activeShift.startCash + activeShift.totalSalesCash);
                  setShowShiftDetails(true);
                }}
                style={{
                  display: 'flex',
                  alignItems: 'center',
                  gap: '8px',
                  padding: '6px 16px',
                  borderRadius: '8px',
                  background: 'rgba(16, 185, 129, 0.15)',
                  border: '1px solid rgba(16, 185, 129, 0.3)',
                  color: '#34d399',
                  cursor: 'pointer',
                  fontWeight: 600,
                  fontSize: '0.8rem'
                }}
              >
                <span className="dot" style={{ width: '8px', height: '8px', borderRadius: '50%', background: '#10b981', display: 'inline-block' }}></span>
                {t('pos.shift_open')}{formatCurrency(activeShift.startCash + activeShift.totalSalesCash)}
              </button>
            ) : (
              <button 
                className="shift-status-btn closed"
                onClick={() => {
                  setStartCash(1000000);
                  setShowShiftDetails(true);
                }}
                style={{
                  display: 'flex',
                  alignItems: 'center',
                  gap: '8px',
                  padding: '6px 16px',
                  borderRadius: '8px',
                  background: 'rgba(239, 68, 68, 0.15)',
                  border: '1px solid rgba(239, 68, 68, 0.3)',
                  color: '#f87171',
                  cursor: 'pointer',
                  fontWeight: 600,
                  fontSize: '0.8rem'
                }}
              >
                <span className="dot" style={{ width: '8px', height: '8px', borderRadius: '50%', background: '#ef4444', display: 'inline-block' }}></span>
                {t('pos.shift_closed')}
              </button>
            )}
          </div>
        )}
      </div>

      {activeTab === 'sell' ? (
        <>
          {/* Search and Scan Header */}
          <div className="pos-actions-bar glass-container">
            <div className="search-input-wrapper">
              <Search className="search-icon" size={20} />
              <input 
                type="text" 
                placeholder={t('pos.search_placeholder')} 
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="pos-search-input"
              />
              {searchTerm && (
                <button className="clear-search" onClick={() => setSearchTerm('')}>
                  <X size={16} />
                </button>
              )}
            </div>
            <button className="barcode-scan-btn" onClick={handleBarcodeScan} title="Simulate Barcode Scan">
              <Barcode size={20} />
              <span>{t('pos.scan_barcode')}</span>
            </button>
          </div>

          <div className="pos-workspace">
            {/* Product Catalog Grid */}
            <div className="pos-catalog glass-container">
              <h3 className="workspace-title">{t('menu.pos')}</h3>
              {loading ? (
                <div className="catalog-loading">
                  <div className="spinner"></div>
                </div>
              ) : filteredProducts.length === 0 ? (
                <div className="empty-catalog">{t('common.no_data')}</div>
              ) : (
                <div className="product-grid">
                  {filteredProducts.map(p => (
                    <div key={p.id} className="product-card" onClick={() => addToCart(p)}>
                      <div className="product-card-img">
                        {p.imageUrl ? (
                          <img src={p.imageUrl} alt={p.name} />
                        ) : (
                          <div className="img-placeholder">
                            <ShoppingCart size={32} />
                          </div>
                        )}
                      </div>
                      <div className="product-card-info">
                        <span className="p-code">{p.code}</span>
                        <h4 className="p-name">{p.name}</h4>
                        <div className="p-footer">
                          <span className="p-price">{formatCurrency(p.price)}</span>
                          <span className="p-stock">{t('pos.stock')}: {p.stockQuantity}</span>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>

            {/* Shopping Cart Drawer */}
            <div className="pos-cart-panel glass-container">
              <div className="cart-header">
                <div className="cart-title-wrapper">
                  <ShoppingCart size={20} className="cart-logo" />
                  <h4>{t('pos.cart')}</h4>
                </div>
                {cart.length > 0 && (
                  <button className="cart-clear-btn" onClick={clearCart}>
                    <Trash2 size={16} />
                  </button>
                )}
              </div>

              <div className="cart-items-list">
                {cart.length === 0 ? (
                  <div className="empty-cart-state">
                    <ShoppingCart size={48} className="empty-cart-icon" />
                    <p>{t('pos.empty_cart')}</p>
                  </div>
                ) : (
                  cart.map(item => (
                    <div key={item.product.id} className="cart-item">
                      <div className="cart-item-info">
                        <span className="cart-item-name">{item.product.name}</span>
                        <span className="cart-item-price">{formatCurrency(item.product.price)}</span>
                      </div>
                      <div className="cart-item-controls">
                        <button className="qty-btn" onClick={() => updateQuantity(item.product.id, -1)}>
                          <Minus size={14} />
                        </button>
                        <span className="qty-val">{item.quantity}</span>
                        <button className="qty-btn" onClick={() => updateQuantity(item.product.id, 1)}>
                          <Plus size={14} />
                        </button>
                        <button className="cart-item-delete" onClick={() => removeFromCart(item.product.id)}>
                          <Trash2 size={14} />
                        </button>
                      </div>
                    </div>
                  ))
                )}
              </div>

              {/* CRM Integration Section */}
              {cart.length > 0 && (
                <div className="pos-crm-section">
                  <div className="crm-search-row">
                    <User size={16} className="cart-logo" />
                    <span style={{ fontSize: '0.8rem', fontWeight: 600, color: 'var(--text-primary)' }}>{t('pos.customer_member')}</span>
                  </div>
                  {!selectedCustomer ? (
                    <div style={{ display: 'flex', flexDirection: 'column', gap: '8px', width: '100%' }}>
                      <div className="crm-search-row">
                        <input 
                          type="text" 
                          placeholder={t('pos.customer_search_placeholder')} 
                          value={customerPhone}
                          onChange={(e) => {
                            setCustomerPhone(e.target.value);
                            setShowCreateCustomerForm(false);
                          }}
                          className="crm-search-input"
                        />
                        <button className="crm-search-btn" onClick={handleSearchCustomer} disabled={crmSearching}>
                          {crmSearching ? '...' : t('common.search')}
                        </button>
                      </div>

                      {showCreateCustomerForm && (
                        <div className="crm-quick-create-box" style={{ padding: '12px', background: 'var(--card-inset)', borderRadius: '6px', border: '1px dashed var(--card-border)', marginTop: '4px' }}>
                          <p style={{ fontSize: '0.75rem', color: '#fbbf24', marginBottom: '8px', fontWeight: 500 }}>
                            {t('pos.customer_not_found_prompt', { defaultValue: '⚠️ Số điện thoại chưa đăng ký. Tạo mới:' })}
                          </p>
                          <div style={{ display: 'flex', gap: '6px' }}>
                            <input 
                              type="text" 
                              placeholder={t('crm.fullname')} 
                              value={newCustomerName}
                              onChange={(e) => setNewCustomerName(e.target.value)}
                              style={{ flex: 1, padding: '6px 8px', borderRadius: '4px', border: '1px solid var(--card-border)', background: 'var(--input-bg)', color: 'var(--text-primary)', fontSize: '0.8rem' }}
                            />
                            <button 
                              onClick={handleCreateCustomer} 
                              disabled={createCustomerLoading}
                              style={{ padding: '6px 12px', background: '#3b82f6', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer', fontSize: '0.8rem', fontWeight: 600 }}
                            >
                              {createCustomerLoading ? '...' : t('pos.btn_create', { defaultValue: 'TẠO' })}
                            </button>
                          </div>
                        </div>
                      )}
                    </div>
                  ) : (
                    <>
                      <div className="pos-customer-badge">
                        <div>
                          <p className="cust-badge-name">{selectedCustomer.name}</p>
                          <p className="cust-badge-points">{t('crm.points')}: {selectedCustomer.loyaltyPoints}đ</p>
                        </div>
                        <button className="cust-badge-remove" onClick={removeCustomerSelection}>
                          <X size={14} />
                        </button>
                      </div>
                      {selectedCustomer.loyaltyPoints > 0 && (
                        <div className="points-redeem-row">
                          <input 
                            type="checkbox" 
                            id="redeemPointsCheckbox"
                            checked={redeemPoints}
                            onChange={(e) => setRedeemPoints(e.target.checked)}
                            className="points-redeem-checkbox"
                          />
                          <label htmlFor="redeemPointsCheckbox" style={{ cursor: 'pointer' }}>
                            {t('pos.use_points', { defaultValue: 'Dùng điểm thưởng giảm giá' })} ({formatCurrency(selectedCustomer.loyaltyPoints * 1000)})
                          </label>
                        </div>
                      )}
                    </>
                  )}
                </div>
              )}
              {/* Mã giảm giá (Coupon Code) Section */}
              {cart.length > 0 && (
                <div className="pos-crm-section" style={{ borderTop: '1px solid var(--card-border)', paddingTop: '12px', marginTop: '12px' }}>
                  <div className="crm-search-row" style={{ marginBottom: '8px' }}>
                    <QrCode size={16} className="cart-logo" />
                    <span style={{ fontSize: '0.8rem', fontWeight: 600, color: 'var(--text-primary)' }}>{t('pos.coupon_apply')}</span>
                  </div>
                  <div className="crm-search-row">
                    <input 
                      type="text" 
                      placeholder={t('pos.coupon_placeholder')} 
                      value={couponCode}
                      onChange={(e) => setCouponCode(e.target.value)}
                      className="crm-search-input"
                      style={{ textTransform: 'uppercase' }}
                    />
                    <button className="crm-search-btn" onClick={handleApplyCoupon}>
                      {t('pos.coupon_btn')}
                    </button>
                  </div>
                  {couponMessage && (
                    <p style={{ 
                      fontSize: '0.75rem', 
                      color: appliedCoupon ? '#34d399' : '#f87171', 
                      marginTop: '6px', 
                      fontWeight: 500 
                    }}>
                      {couponMessage}
                    </p>
                  )}
                </div>
              )}

              {cart.length > 0 && (
                <div className="cart-summary">
                  {getDiscountAmount() > 0 && (
                    <div className="summary-row">
                      <span>{t('pos.customer_discount', { defaultValue: 'Chiết khấu khách hàng:' })}</span>
                      <span style={{ color: '#f87171' }}>-{formatCurrency(getDiscountAmount())}</span>
                    </div>
                  )}
                  {getCouponDiscount() > 0 && (
                    <div className="summary-row">
                      <span>{t('pos.coupon_code_applied', { defaultValue: 'Mã giảm giá' })} ({appliedCoupon?.code}):</span>
                      <span style={{ color: '#f87171' }}>-{formatCurrency(getCouponDiscount())}</span>
                    </div>
                  )}
                  <div className="summary-row total-row">
                    <span>{t('pos.total')}:</span>
                    <span className="grand-total">{formatCurrency(getFinalTotal())}</span>
                  </div>
                  <button className="pos-checkout-btn" onClick={() => setIsCheckoutOpen(true)}>
                    <span>{t('pos.checkout')}</span>
                  </button>
                </div>
              )}
            </div>
          </div>
        </>
      ) : (
        <div className="recent-orders-list glass-container" style={{ padding: '24px' }}>
          <h3 className="workspace-title" style={{ marginBottom: '16px' }}>{t('pos.recent_branch_orders')}</h3>
          {recentOrdersLoading ? (
            <div className="catalog-loading">
              <div className="spinner"></div>
            </div>
          ) : recentOrders.length === 0 ? (
            <div className="empty-catalog">{t('pos.empty_branch_orders')}</div>
          ) : (
            <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
              {recentOrders.map(o => (
                <div key={o.id} className="recent-order-card">
                  <div className="ro-info">
                    <div className="ro-header">
                      <span className="ro-code">{o.orderCode}</span>
                      <span className="ro-date">{new Date(o.createdAt).toLocaleString('vi-VN')}</span>
                    </div>
                    <div style={{ display: 'flex', gap: '15px', marginTop: '4px', fontSize: '0.8rem' }}>
                      <span className="ro-amt">{t('pos.total')}: {formatCurrency(o.finalAmount)}</span>
                      {o.discount > 0 && <span style={{ color: '#f87171' }}>{t('pos.discount_label', { defaultValue: 'Giảm' })}: {formatCurrency(o.discount)}</span>}
                      {o.customer && <span style={{ color: '#60a5fa' }}>{t('dashboard.th_customer')}: {o.customer.name}</span>}
                    </div>
                  </div>
                  <div className="ro-actions" style={{ display: 'flex', gap: '8px', alignItems: 'center' }}>
                    <span className={`ro-status-badge ${o.status === 2 ? 'ro-status-paid' : 'ro-status-draft'}`}>
                      {o.status === 2 ? t('dashboard.status_completed') : t('dashboard.status_pending')}
                    </span>
                    <button className="ro-print-btn" onClick={() => handleRecentOrderPrint(o)}>
                      <Printer size={14} style={{ marginRight: '4px', display: 'inline-block', verticalAlign: 'middle' }} />
                      {t('common.print_invoice', { defaultValue: 'In hóa đơn' })}
                    </button>
                    {o.status === 2 && (
                      <button 
                        className="ro-print-btn" 
                        onClick={() => handleOpenReturnModal(o)}
                        style={{ background: 'rgba(239, 68, 68, 0.1)', borderColor: 'rgba(239, 68, 68, 0.3)', color: '#f87171' }}
                      >
                        {t('common.return_goods', { defaultValue: 'Trả hàng' })}
                      </button>
                    )}
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      )}

      {/* Checkout Modal */}
      {isCheckoutOpen && (
        <div className="modal-overlay">
          <div className="modal-content glass-container">
            <div className="modal-header">
              <h3>{t('pos.checkout')}</h3>
              <button className="modal-close" onClick={() => setIsCheckoutOpen(false)}>
                <X size={20} />
              </button>
            </div>

            <div className="modal-body">
              {!checkoutResult ? (
                <>
                  <div className="checkout-payment-methods">
                    <p className="method-label">{t('pos.payment_method')}</p>
                    <div className="methods-grid">
                      <button 
                        className={`method-card ${selectedMethod === 'Cash' ? 'active' : ''}`}
                        onClick={() => setSelectedMethod('Cash')}
                      >
                        <DollarSign size={24} />
                        <span>{t('pos.cash')}</span>
                      </button>

                      <button 
                        className={`method-card ${selectedMethod === 'VnPay' ? 'active' : ''}`}
                        onClick={() => setSelectedMethod('VnPay')}
                      >
                        <Wallet size={24} className="vnpay-icon" />
                        <span>{t('pos.vnpay')}</span>
                      </button>

                      <button 
                        className={`method-card ${selectedMethod === 'MoMo' ? 'active' : ''}`}
                        onClick={() => setSelectedMethod('MoMo')}
                      >
                        <Wallet size={24} className="momo-icon" />
                        <span>{t('pos.momo')}</span>
                      </button>

                      <button 
                        className={`method-card ${selectedMethod === 'VietQr' ? 'active' : ''}`}
                        onClick={() => setSelectedMethod('VietQr')}
                      >
                        <QrCode size={24} />
                        <span>{t('pos.vietqr')}</span>
                      </button>
                    </div>
                  </div>

                  <div className="checkout-total-bill">
                    <span>{t('crm.total_invoice')}:</span>
                    <span className="bill-val">{formatCurrency(getFinalTotal())}</span>
                  </div>

                  <button 
                    className="modal-submit-btn" 
                    onClick={handleCheckoutSubmit}
                    disabled={checkoutLoading}
                  >
                    {checkoutLoading ? (
                      <>
                        <span className="btn-spinner"></span>
                        {t('login.loading')}
                      </>
                    ) : (
                      t('common.confirm')
                    )}
                  </button>
                </>
              ) : (
                <div className="checkout-success-view">
                  <CheckCircle2 size={54} className="success-badge-icon" />
                  <h4>{t('pos.payment_success')}</h4>
                  <p>{checkoutResult.paid ? t('pos.checkout_completed_desc') : t('pos.order_created')}</p>
                  
                  {realtimePaidAlert && (
                    <div className="glass-container" style={{ background: 'rgba(16, 185, 129, 0.15)', border: '1px solid #10b981', padding: '10px 16px', borderRadius: '8px', color: '#34d399', fontWeight: 600 }}>
                      {t('pos.realtime_paid_alert')}
                    </div>
                  )}

                  <div className="receipt-details">
                    <div className="receipt-row">
                      <span>{t('pos.order_code_lbl')}</span>
                      <strong>{checkoutResult.orderCode}</strong>
                    </div>
                    <div className="receipt-row">
                      <span>{t('pos.total_lbl')}</span>
                      <strong className="receipt-amount">{formatCurrency(checkoutResult.totalAmount)}</strong>
                    </div>
                    <div className="receipt-row">
                      <span>{t('pos.method_lbl')}</span>
                      <span>{selectedMethod}</span>
                    </div>
                  </div>

                  {/* Payment link for VNPAY/MoMo */}
                  {checkoutResult.paymentUrl && !checkoutResult.paid && (
                    <div className="gateway-action-container">
                      <p>{t('pos.pay_link')} {selectedMethod.toUpperCase()}</p>
                      <a href={checkoutResult.paymentUrl} target="_blank" rel="noopener noreferrer" className="gateway-link">
                        <span>{t('pos.continue_payment')}</span>
                        <ExternalLink size={16} />
                      </a>
                    </div>
                  )}

                  {/* VietQR dynamic display */}
                  {selectedMethod === 'VietQr' && !checkoutResult.paid && (
                    <div className="vietqr-display-container">
                      <p className="qr-title">{t('pos.vietqr_title')}</p>
                      <div className="qr-box">
                        <img 
                          src={`https://img.vietqr.io/image/970415-1131130600099999-compact2.png?amount=${checkoutResult.totalAmount}&addInfo=${checkoutResult.orderCode}`} 
                          alt="VietQR Payment Code" 
                          className="vietqr-image"
                        />
                      </div>
                      <p className="qr-helper">{t('pos.scan_to_pay')}</p>
                      <div style={{ display: 'flex', gap: '8px', alignItems: 'center', fontSize: '0.75rem', color: 'var(--text-secondary)' }}>
                        <span className="btn-spinner" style={{ width: '12px', height: '12px' }}></span>
                        <span>{t('pos.awaiting_bank_signal')}</span>
                      </div>
                    </div>
                  )}

                  <button className="finish-checkout-btn" onClick={handleFinishPayment}>
                    {t('pos.finish_print')}
                  </button>
                </div>
              )}
            </div>
          </div>
        </div>
      )}

      {/* Thermal Print Preview Modal */}
      {isReceiptModalOpen && receiptOrder && (
        <div className="modal-overlay">
          <div className="modal-content glass-container" style={{ maxWidth: '420px', padding: '20px' }}>
            <div className="modal-header">
              <h3 style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                <Printer size={20} />
                {t('pos.receipt_k80')}
              </h3>
              <button className="modal-close" onClick={() => setIsReceiptModalOpen(false)}>
                <X size={20} />
              </button>
            </div>
            
            <div className="thermal-receipt-container">
              <div className="thermal-receipt" id="thermal-receipt-print-area">
                <div className="receipt-header-print">
                  <h2>SALESCRM RETAIL</h2>
                  <p>Hệ thống Cửa hàng Toàn Quốc</p>
                  <p>Chi nhánh: {branchId ? "Chi nhánh Cửa hàng" : "Trung tâm HQ"}</p>
                  <p>SĐT: 024-3999-8888</p>
                </div>
                
                <div className="receipt-divider"></div>
                
                <div className="receipt-info-block">
                  <div className="receipt-info-row">
                    <span>Số HĐ:</span>
                    <strong>{receiptOrder.orderCode}</strong>
                  </div>
                  <div className="receipt-info-row">
                    <span>Ngày:</span>
                    <span>{receiptOrder.createdAt}</span>
                  </div>
                  {receiptOrder.customerName && (
                    <div className="receipt-info-row">
                      <span>Khách hàng:</span>
                      <span>{receiptOrder.customerName}</span>
                    </div>
                  )}
                </div>
                
                <div className="receipt-divider"></div>
                
                <table className="receipt-items-table">
                  <thead>
                    <tr>
                      <th style={{ textAlign: 'left' }}>Sản phẩm</th>
                      <th style={{ textAlign: 'center' }}>SL</th>
                      <th style={{ textAlign: 'right' }}>Thành tiền</th>
                    </tr>
                  </thead>
                  <tbody>
                    {receiptOrder.items.map((it: any, index: number) => (
                      <tr key={index}>
                        <td style={{ textAlign: 'left' }}>
                          {it.name}<br />
                          <small style={{ color: '#555' }}>{it.quantity} x {formatCurrency(it.price)}</small>
                        </td>
                        <td style={{ textAlign: 'center' }}>{it.quantity}</td>
                        <td style={{ textAlign: 'right' }}>{formatCurrency(it.price * it.quantity)}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
                
                <div className="receipt-divider"></div>
                
                <div className="receipt-summary-box">
                  <div className="receipt-summary-row">
                    <span>Cộng tiền hàng:</span>
                    <span>{formatCurrency(receiptOrder.subtotal)}</span>
                  </div>
                  {receiptOrder.discount > 0 && (
                    <div className="receipt-summary-row">
                      <span>Giảm giá:</span>
                      <span>-{formatCurrency(receiptOrder.discount)}</span>
                    </div>
                  )}
                  <div className="receipt-summary-row bold-row">
                    <span>TỔNG CỘNG:</span>
                    <span>{formatCurrency(receiptOrder.finalAmount)}</span>
                  </div>
                  <div className="receipt-summary-row" style={{ marginTop: '4px' }}>
                    <span>Thanh toán:</span>
                    <span>{receiptOrder.paymentMethod}</span>
                  </div>
                </div>
                
                <div className="receipt-divider"></div>
                
                <div className="receipt-footer-print">
                  <p>CẢM ƠN QUÝ KHÁCH & HẸN GẶP LẠI!</p>
                  <p>Powered by SalesCRM</p>
                </div>
              </div>
            </div>

            <div style={{ display: 'flex', gap: '10px', marginTop: '12px' }}>
              <button 
                className="finish-checkout-btn" 
                onClick={() => {
                  // Simulate system print window trigger
                  window.print();
                }}
                style={{ flex: 1, background: '#3b82f6', borderColor: '#3b82f6', fontWeight: 600 }}
              >
                {t('common.print_direct', { defaultValue: 'In trực tiếp' })}
              </button>
              <button 
                className="finish-checkout-btn" 
                onClick={() => setIsReceiptModalOpen(false)}
                style={{ flex: 1 }}
              >
                {t('common.cancel')}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Shift Management / Details Modal */}
      {showShiftDetails && (
        <div className="modal-overlay">
          <div className="modal-content glass-container" style={{ maxWidth: '460px', padding: '24px' }}>
            <div className="modal-header">
              <h3>{activeShift ? "Quản Lý Ca Làm Việc" : "Mở Ca Làm Việc Mới"}</h3>
              <button className="modal-close" onClick={() => setShowShiftDetails(false)}>
                <X size={20} />
              </button>
            </div>
            
            <div className="modal-body" style={{ color: 'var(--text-primary)' }}>
              {activeShift ? (
                <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
                  <div className="shift-info-grid" style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '12px', fontSize: '0.85rem' }}>
                    <div style={{ padding: '8px', background: 'var(--card-inset)', borderRadius: '6px' }}>
                      <span style={{ color: 'var(--text-secondary)', display: 'block', fontSize: '0.75rem' }}>Nhân viên:</span>
                      <strong>{useAuthStore.getState().fullName}</strong>
                    </div>
                    <div style={{ padding: '8px', background: 'var(--card-inset)', borderRadius: '6px' }}>
                      <span style={{ color: 'var(--text-secondary)', display: 'block', fontSize: '0.75rem' }}>Giờ bắt đầu:</span>
                      <strong>{new Date(activeShift.startTime).toLocaleTimeString('vi-VN')} {new Date(activeShift.startTime).toLocaleDateString('vi-VN')}</strong>
                    </div>
                    <div style={{ padding: '8px', background: 'var(--card-inset)', borderRadius: '6px' }}>
                      <span style={{ color: 'var(--text-secondary)', display: 'block', fontSize: '0.75rem' }}>Tiền mặt đầu ca:</span>
                      <strong>{formatCurrency(activeShift.startCash)}</strong>
                    </div>
                    <div style={{ padding: '8px', background: 'var(--card-inset)', borderRadius: '6px' }}>
                      <span style={{ color: 'var(--text-secondary)', display: 'block', fontSize: '0.75rem' }}>Doanh số Tiền mặt:</span>
                      <strong>{formatCurrency(activeShift.totalSalesCash)}</strong>
                    </div>
                    <div style={{ padding: '8px', background: 'var(--card-inset)', borderRadius: '6px' }}>
                      <span style={{ color: 'var(--text-secondary)', display: 'block', fontSize: '0.75rem' }}>VietQR (Chuyển khoản):</span>
                      <strong>{formatCurrency(activeShift.totalSalesBank)}</strong>
                    </div>
                    <div style={{ padding: '8px', background: 'var(--card-inset)', borderRadius: '6px' }}>
                      <span style={{ color: 'var(--text-secondary)', display: 'block', fontSize: '0.75rem' }}>VNPay:</span>
                      <strong>{formatCurrency(activeShift.totalSalesVNPay)}</strong>
                    </div>
                    <div style={{ padding: '8px', background: 'var(--card-inset)', borderRadius: '6px' }}>
                      <span style={{ color: 'var(--text-secondary)', display: 'block', fontSize: '0.75rem' }}>MoMo:</span>
                      <strong>{formatCurrency(activeShift.totalSalesMomo)}</strong>
                    </div>
                    <div style={{ padding: '8px', background: 'rgba(16, 185, 129, 0.08)', borderRadius: '6px', border: '1px solid rgba(16, 185, 129, 0.15)' }}>
                      <span style={{ color: '#34d399', display: 'block', fontSize: '0.75rem' }}>Tiền mặt két dự kiến:</span>
                      <strong style={{ color: '#34d399' }}>{formatCurrency(activeShift.startCash + activeShift.totalSalesCash)}</strong>
                    </div>
                  </div>

                  <div style={{ borderTop: '1px solid var(--card-border)', paddingTop: '16px', display: 'flex', flexDirection: 'column', gap: '12px' }}>
                    <h4 style={{ fontWeight: 600, fontSize: '0.9rem' }}>{t('pos.close_shift_title')}</h4>
                    <div>
                      <label style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', display: 'block', marginBottom: '6px' }}>
                        {t('pos.cash_counted_lbl')}
                      </label>
                      <input 
                        type="number"
                        value={endCashCounted}
                        onChange={(e) => setEndCashCounted(Number(e.target.value))}
                        style={{
                          width: '100%',
                          padding: '10px 14px',
                          borderRadius: '8px',
                          border: '1px solid var(--card-border)',
                          background: 'var(--input-bg)',
                          color: 'var(--text-primary)',
                          fontSize: '0.95rem',
                          fontWeight: 600
                        }}
                      />
                    </div>
                    <div>
                      <label style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', display: 'block', marginBottom: '6px' }}>
                        {t('pos.shift_notes_lbl')}
                      </label>
                      <textarea
                        rows={2}
                        value={shiftNotes}
                        onChange={(e) => setShiftNotes(e.target.value)}
                        placeholder={t('pos.shift_notes_placeholder')}
                        style={{
                          width: '100%',
                          padding: '10px 14px',
                          borderRadius: '8px',
                          border: '1px solid var(--card-border)',
                          background: 'var(--input-bg)',
                          color: 'var(--text-primary)',
                          fontSize: '0.85rem',
                          resize: 'none'
                        }}
                      />
                    </div>

                    <button 
                      className="finish-checkout-btn" 
                      onClick={handleCloseShift}
                      disabled={closingShiftLoading}
                      style={{ background: '#ef4444', borderColor: '#ef4444', fontWeight: 600, marginTop: '8px' }}
                    >
                      {closingShiftLoading ? t('common.loading') : t('pos.btn_close_shift')}
                    </button>
                  </div>
                </div>
              ) : (
                <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
                  <p style={{ fontSize: '0.85rem', color: 'var(--text-secondary)' }}>
                    {t('pos.open_shift_desc')}
                  </p>
                  <div>
                    <label style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', display: 'block', marginBottom: '6px' }}>
                      {t('pos.start_cash_lbl')}
                    </label>
                    <input 
                      type="number"
                      value={startCash}
                      onChange={(e) => setStartCash(Number(e.target.value))}
                      style={{
                        width: '100%',
                        padding: '10px 14px',
                        borderRadius: '8px',
                        border: '1px solid var(--card-border)',
                        background: 'var(--input-bg)',
                        color: 'var(--text-primary)',
                        fontSize: '0.95rem',
                        fontWeight: 600
                      }}
                    />
                  </div>
                  <button 
                    className="finish-checkout-btn" 
                    onClick={handleOpenShift}
                    disabled={openingShiftLoading}
                    style={{ background: '#3b82f6', borderColor: '#3b82f6', fontWeight: 600 }}
                  >
                    {openingShiftLoading ? t('common.loading') : t('pos.btn_open_shift')}
                  </button>
                </div>
              )}
            </div>
          </div>
        </div>
      )}

      {/* Returns / Refund Modal */}
      {isReturnModalOpen && returningOrder && (
        <div className="modal-overlay">
          <div className="modal-content glass-container" style={{ maxWidth: '520px', padding: '24px' }}>
            <div className="modal-header">
              <h3>{t('pos.return_refund_title')}</h3>
              <button className="modal-close" onClick={() => setIsReturnModalOpen(false)}>
                <X size={20} />
              </button>
            </div>
            
            <div className="modal-body" style={{ color: 'var(--text-primary)' }}>
              <div style={{ marginBottom: '12px', fontSize: '0.85rem' }}>
                <span>{t('pos.original_order')} </span><strong>{returningOrder.orderCode}</strong>
                <span style={{ margin: '0 8px', color: 'var(--text-secondary)' }}>|</span>
                <span>{t('pos.customer')} </span><strong>{returningOrder.customer?.name || t('dashboard.walkin_customer')}</strong>
              </div>

              <div style={{ display: 'flex', flexDirection: 'column', gap: '10px', maxHeight: '200px', overflowY: 'auto', paddingRight: '4px', marginBottom: '16px' }}>
                {returningOrder.orderDetails?.map((d: any) => (
                  <div key={d.productId} style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '8px 12px', background: 'var(--card-inset)', borderRadius: '8px' }}>
                    <div style={{ flex: 1 }}>
                      <p style={{ fontSize: '0.85rem', fontWeight: 500 }}>{d.product?.name || 'Sản phẩm'}</p>
                      <small style={{ color: 'var(--text-secondary)' }}>{formatCurrency(d.unitPrice)} x SL mua: {d.quantity}</small>
                    </div>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                      <span style={{ fontSize: '0.8rem', color: 'var(--text-secondary)' }}>{t('pos.return_qty')}</span>
                      <input 
                        type="number"
                        min={0}
                        max={d.quantity}
                        value={returnQuantities[d.productId] || 0}
                        onChange={(e) => {
                          const val = Math.min(d.quantity, Math.max(0, Number(e.target.value)));
                          setReturnQuantities(prev => ({ ...prev, [d.productId]: val }));
                        }}
                        style={{
                          width: '60px',
                          padding: '6px',
                          borderRadius: '6px',
                          border: '1px solid var(--card-border)',
                          background: 'var(--input-bg)',
                          color: 'var(--text-primary)',
                          textAlign: 'center',
                          fontWeight: 600
                        }}
                      />
                    </div>
                  </div>
                ))}
              </div>

              <div style={{ display: 'flex', flexDirection: 'column', gap: '12px' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '10px', background: 'rgba(239, 68, 68, 0.08)', borderRadius: '8px', border: '1px solid rgba(239,68,68,0.15)' }}>
                  <span style={{ fontSize: '0.85rem', fontWeight: 600 }}>{t('pos.total_refund')}</span>
                  <strong style={{ color: '#f87171', fontSize: '1.1rem' }}>
                    {formatCurrency(
                      returningOrder.orderDetails?.reduce((sum: number, d: any) => {
                        const qty = returnQuantities[d.productId] || 0;
                        return sum + (qty * d.unitPrice);
                      }, 0) || 0
                    )}
                  </strong>
                </div>

                <div>
                  <label style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', display: 'block', marginBottom: '6px' }}>
                    {t('pos.payment_method')}
                  </label>
                  <div style={{ display: 'flex', gap: '8px' }}>
                    {(['Cash', 'BankTransfer', 'MoMo', 'VNPay'] as const).map(m => (
                      <button 
                        key={m}
                        onClick={() => setReturnRefundMethod(m)}
                        style={{
                          flex: 1,
                          padding: '8px',
                          borderRadius: '6px',
                          border: '1px solid',
                          borderColor: returnRefundMethod === m ? '#ef4444' : 'var(--card-border)',
                          background: returnRefundMethod === m ? 'rgba(239, 68, 68, 0.1)' : 'var(--input-bg)',
                          color: returnRefundMethod === m ? '#f87171' : 'var(--text-primary)',
                          fontSize: '0.8rem',
                          fontWeight: 600,
                          cursor: 'pointer'
                        }}
                      >
                        {m === 'Cash' ? t('pos.cash') : m === 'BankTransfer' ? t('pos.vietqr') : m}
                      </button>
                    ))}
                  </div>
                </div>

                <div>
                  <label style={{ fontSize: '0.8rem', color: 'var(--text-secondary)', display: 'block', marginBottom: '6px' }}>
                    {t('pos.return_reason')}
                  </label>
                  <textarea
                    rows={2}
                    value={returnReason}
                    onChange={(e) => setReturnReason(e.target.value)}
                    placeholder={t('pos.return_reason_placeholder')}
                    style={{
                      width: '100%',
                      padding: '10px 14px',
                      borderRadius: '8px',
                      border: '1px solid var(--card-border)',
                      background: 'var(--input-bg)',
                      color: 'var(--text-primary)',
                      fontSize: '0.85rem',
                      resize: 'none'
                    }}
                  />
                </div>

                {returnError && (
                  <p style={{ color: '#f87171', fontSize: '0.8rem', fontWeight: 500 }}>⚠️ {returnError}</p>
                )}

                <button 
                  className="finish-checkout-btn" 
                  onClick={handleReturnSubmit}
                  disabled={submittingReturn}
                  style={{ background: '#ef4444', borderColor: '#ef4444', fontWeight: 600, marginTop: '4px' }}
                >
                  {submittingReturn ? t('common.loading') : t('pos.btn_confirm_return')}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
