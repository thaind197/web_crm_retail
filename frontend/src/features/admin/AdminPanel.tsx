import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { useAuthStore } from '../../store/useAuthStore';
import { 
  Building2, 
  Users, 
  Tag, 
  Plus, 
  Edit, 
  Trash2, 
  Save, 
  UserPlus, 
  Barcode, 
  Upload, 
  CheckCircle,
  AlertTriangle,
  X
} from 'lucide-react';
import axios from 'axios';
import { API_BASE_URL } from '../../config';
import './AdminPanel.css';

interface Branch {
  id: string;
  name: string;
  address?: string;
  phone?: string;
  isActive: boolean;
}

interface UserDto {
  id: string;
  userName: string;
  fullName: string;
  email: string;
  roles: string[];
  branchId?: string;
  branchName?: string;
  isActive: boolean;
}

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

export const AdminPanel: React.FC = () => {
  const { t } = useTranslation();
  const { token } = useAuthStore();
  
  // Tab control: 'products' | 'users' | 'branches'
  const [activeSubTab, setActiveSubTab] = useState<'products' | 'users' | 'branches'>('products');

  // Common notification
  const [notice, setNotice] = useState<{ type: 'success' | 'error', text: string } | null>(null);

  const triggerNotice = (type: 'success' | 'error', text: string) => {
    setNotice({ type, text });
    setTimeout(() => setNotice(null), 4000);
  };

  // ==========================================
  // 1. PRODUCTS MANAGEMENT STATES & ACTIONS
  // ==========================================
  const [products, setProducts] = useState<Product[]>([]);
  const [productsLoading, setProductsLoading] = useState(false);
  const [editingProduct, setEditingProduct] = useState<Partial<Product> | null>(null);
  const [isProductFormOpen, setIsProductFormOpen] = useState(false);
  const [productImageBase64, setProductImageBase64] = useState<string | undefined>(undefined);

  const fetchProducts = async () => {
    setProductsLoading(true);
    try {
      const res = await axios.get(`${API_BASE_URL}/api/products?pageSize=50`, {
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
      console.warn("Failed fetching products", err);
      setProducts([]);
    } finally {
      setProductsLoading(false);
    }
  };

  const handleGenerateBarcode = () => {
    const randomSuffix = Math.floor(1000000000 + Math.random() * 9000000000);
    const barcodeVal = '893' + randomSuffix;
    setEditingProduct(prev => prev ? { ...prev, barcode: barcodeVal } : { barcode: barcodeVal });
  };

  const handleProductImageUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      const reader = new FileReader();
      reader.onloadend = () => {
        setProductImageBase64(reader.result as string);
        setEditingProduct(prev => prev ? { ...prev, imageUrl: reader.result as string } : {});
      };
      reader.readAsDataURL(file);
    }
  };

  const handleSaveProduct = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingProduct?.name || !editingProduct?.price) {
      triggerNotice('error', 'Vui lòng nhập đầy đủ thông tin sản phẩm bắt buộc.');
      return;
    }

    try {
      if (editingProduct.id) {
        // Edit product
        await axios.put(`${API_BASE_URL}/api/products/${editingProduct.id}`, {
          id: editingProduct.id,
          name: editingProduct.name,
          code: editingProduct.code || null,
          description: null,
          sellingPrice: Number(editingProduct.price),
          importPrice: Number(editingProduct.importPrice || Math.round(Number(editingProduct.price) * 0.7)),
          barcode: editingProduct.barcode || 'N/A',
          imageUrl: productImageBase64 ? null : (editingProduct.imageUrl || null),
          imageBase64: productImageBase64 || null,
          isActive: editingProduct.isActive !== false
        }, { headers: { Authorization: `Bearer ${token}` } });
        triggerNotice('success', 'Cập nhật sản phẩm thành công!');
      } else {
        // Create product
        await axios.post(`${API_BASE_URL}/api/products`, {
          name: editingProduct.name,
          code: null, // Let backend auto-generate code
          description: null,
          sellingPrice: Number(editingProduct.price),
          importPrice: Number(editingProduct.importPrice || Math.round(Number(editingProduct.price) * 0.7)),
          barcode: editingProduct.barcode || 'N/A',
          imageUrl: null,
          imageBase64: productImageBase64 || null,
          isActive: true
        }, { headers: { Authorization: `Bearer ${token}` } });
        triggerNotice('success', 'Thêm sản phẩm thành công!');
      }
      setIsProductFormOpen(false);
      setEditingProduct(null);
      setProductImageBase64(undefined);
      fetchProducts();
    } catch (err: any) {
      console.error(err);
      triggerNotice('error', err.response?.data?.message || 'Có lỗi xảy ra khi lưu sản phẩm.');
    }
  };

  const handleDeleteProduct = async (id: string) => {
    if (!window.confirm('Bạn có chắc muốn xóa sản phẩm này không?')) return;
    try {
      await axios.delete(`${API_BASE_URL}/api/products/${id}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      triggerNotice('success', 'Xóa sản phẩm thành công!');
      fetchProducts();
    } catch (err: any) {
      triggerNotice('error', 'Không thể xóa sản phẩm. Hãy đảm bảo sản phẩm chưa có lịch sử bán hàng.');
    }
  };

  // ==========================================
  // 2. USERS MANAGEMENT STATES & ACTIONS
  // ==========================================
  const [users, setUsers] = useState<UserDto[]>([]);
  const [usersLoading, setUsersLoading] = useState(false);
  const [isUserFormOpen, setIsUserFormOpen] = useState(false);
  const [editingUser, setEditingUser] = useState<any>({ userName: '', fullName: '', email: '', password: '', role: 'Staff', branchId: '' });

  const fetchUsers = async () => {
    setUsersLoading(true);
    try {
      const res = await axios.get(`${API_BASE_URL}/api/users`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (res.data.isSuccess && res.data.data?.items) {
        const mapped = res.data.data.items.map((u: any) => ({
          id: u.id,
          userName: u.username || u.userName || '',
          fullName: u.fullName || '',
          email: u.email || '',
          roles: u.roles || (u.role ? [u.role] : []),
          branchId: u.branchId || '',
          branchName: u.branchName || '',
          isActive: u.isActive !== undefined ? u.isActive : true
        }));
        setUsers(mapped);
      }
    } catch (err) {
      console.warn("Failed fetching users", err);
      setUsers([]);
    } finally {
      setUsersLoading(false);
    }
  };

  const handleSaveUser = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingUser.userName || !editingUser.fullName || !editingUser.email || (!editingUser.id && !editingUser.password)) {
      triggerNotice('error', 'Vui lòng cung cấp các trường thông tin nhân sự bắt buộc.');
      return;
    }

    try {
      if (editingUser.id) {
        // Update user
        await axios.put(`${API_BASE_URL}/api/users/${editingUser.id}`, {
          id: editingUser.id,
          fullName: editingUser.fullName,
          email: editingUser.email,
          branchId: editingUser.branchId || null,
          role: editingUser.role,
          isActive: editingUser.isActive !== undefined ? editingUser.isActive : true,
          password: editingUser.password || null
        }, { headers: { Authorization: `Bearer ${token}` } });
        triggerNotice('success', 'Cập nhật nhân viên thành công.');
      } else {
        // Create user
        await axios.post(`${API_BASE_URL}/api/users`, {
          username: editingUser.userName,
          fullName: editingUser.fullName,
          email: editingUser.email,
          password: editingUser.password,
          role: editingUser.role,
          branchId: editingUser.branchId || null
        }, { headers: { Authorization: `Bearer ${token}` } });
        triggerNotice('success', 'Thêm nhân viên thành công.');
      }
      setIsUserFormOpen(false);
      setEditingUser({ userName: '', fullName: '', email: '', password: '', role: 'Staff', branchId: '' });
      fetchUsers();
    } catch (err: any) {
      triggerNotice('error', err.response?.data?.message || 'Lỗi lưu thông tin nhân viên.');
    }
  };

  const handleDeleteUser = async (id: string) => {
    if (!window.confirm('Bạn có chắc muốn xóa/vô hiệu hóa nhân sự này?')) return;
    try {
      await axios.delete(`${API_BASE_URL}/api/users/${id}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      triggerNotice('success', 'Đã xóa nhân viên thành công.');
      fetchUsers();
    } catch (err) {
      triggerNotice('error', 'Có lỗi xảy ra khi xóa nhân viên.');
    }
  };

  // ==========================================
  // 3. BRANCHES MANAGEMENT STATES & ACTIONS
  // ==========================================
  const [branches, setBranches] = useState<Branch[]>([]);
  const [branchesLoading, setBranchesLoading] = useState(false);
  const [editingBranch, setEditingBranch] = useState<Partial<Branch> | null>(null);
  const [isBranchFormOpen, setIsBranchFormOpen] = useState(false);

  const fetchBranches = async () => {
    setBranchesLoading(true);
    try {
      const res = await axios.get(`${API_BASE_URL}/api/branches`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (res.data.isSuccess && res.data.data) {
        setBranches(res.data.data);
      }
    } catch (err) {
      console.warn("Failed fetching branches", err);
      setBranches([]);
    } finally {
      setBranchesLoading(false);
    }
  };

  const handleSaveBranch = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!editingBranch?.name) {
      triggerNotice('error', 'Tên chi nhánh không được để trống.');
      return;
    }

    try {
      if (editingBranch.id) {
        // Update Branch
        await axios.put(`${API_BASE_URL}/api/branches/${editingBranch.id}`, {
          name: editingBranch.name,
          address: editingBranch.address,
          phone: editingBranch.phone,
          isActive: editingBranch.isActive ?? true
        }, { headers: { Authorization: `Bearer ${token}` } });
        triggerNotice('success', 'Cập nhật chi nhánh thành công.');
      } else {
        // Create Branch
        await axios.post(`${API_BASE_URL}/api/branches`, {
          name: editingBranch.name,
          address: editingBranch.address,
          phone: editingBranch.phone,
          isActive: true
        }, { headers: { Authorization: `Bearer ${token}` } });
        triggerNotice('success', 'Thêm chi nhánh thành công.');
      }
      setIsBranchFormOpen(false);
      setEditingBranch(null);
      fetchBranches();
    } catch (err: any) {
      triggerNotice('error', err.response?.data?.message || 'Lỗi khi lưu chi nhánh.');
    }
  };

  const handleDeleteBranch = async (id: string) => {
    if (!window.confirm('Bạn có chắc muốn xóa chi nhánh này?')) return;
    try {
      await axios.delete(`${API_BASE_URL}/api/branches/${id}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      triggerNotice('success', 'Xóa chi nhánh thành công.');
      fetchBranches();
    } catch (err: any) {
      triggerNotice('error', err.response?.data?.message || 'Không thể xóa chi nhánh có giao dịch liên quan.');
    }
  };

  // Initial loads
  useEffect(() => {
    if (activeSubTab === 'products') fetchProducts();
    if (activeSubTab === 'users') {
      fetchUsers();
      fetchBranches(); // Fetch branches for user selection dropdown
    }
    if (activeSubTab === 'branches') fetchBranches();
  }, [token, activeSubTab]);

  const formatCurrency = (val: number) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val);
  };

  return (
    <div className="admin-container">
      {/* Notifications banner */}
      {notice && (
        <div className={`notice-banner ${notice.type === 'success' ? 'notice-success' : 'notice-error'}`}>
          {notice.type === 'success' ? <CheckCircle size={18} /> : <AlertTriangle size={18} />}
          <span>{notice.text}</span>
        </div>
      )}

      {/* Header Tabs */}
      <div className="admin-tabs-nav glass-container">
        <button 
          className={`admin-tab-btn ${activeSubTab === 'products' ? 'active' : ''}`}
          onClick={() => setActiveSubTab('products')}
        >
          <Tag size={16} />
          <span>{t('admin.tab_products', { defaultValue: 'SẢN PHẨM' })}</span>
        </button>

        <button 
          className={`admin-tab-btn ${activeSubTab === 'users' ? 'active' : ''}`}
          onClick={() => setActiveSubTab('users')}
        >
          <Users size={16} />
          <span>{t('admin.tab_users', { defaultValue: 'NHÂN SỰ' })}</span>
        </button>

        <button 
          className={`admin-tab-btn ${activeSubTab === 'branches' ? 'active' : ''}`}
          onClick={() => setActiveSubTab('branches')}
        >
          <Building2 size={16} />
          <span>{t('admin.tab_branches', { defaultValue: 'CHI NHÁNH' })}</span>
        </button>
      </div>

      {/* ==========================================
          PRODUCT CRUD TAB
          ========================================== */}
      {activeSubTab === 'products' && (
        <div className="admin-panel-content glass-container">
          <div className="panel-header">
            <h3>{t('admin.products_header', { defaultValue: 'Danh Sách Sản Phẩm Hệ Thống' })}</h3>
            <button className="panel-action-btn" onClick={() => {
              setEditingProduct({ name: '', code: '', price: 0, barcode: '' });
              setProductImageBase64(undefined);
              setIsProductFormOpen(true);
            }}>
              <Plus size={16} />
              {t('admin.add_product', { defaultValue: 'Thêm sản phẩm mới' })}
            </button>
          </div>

          {productsLoading ? (
            <div className="spinner-wrapper"><div className="spinner"></div></div>
          ) : (
            <div className="admin-table-container">
              <table className="admin-table">
                <thead>
                  <tr>
                    <th>{t('admin.th_image')}</th>
                    <th>{t('admin.th_product_code')}</th>
                    <th>{t('admin.th_product_name')}</th>
                    <th>{t('admin.th_price')}</th>
                    <th>{t('admin.th_barcode')}</th>
                    <th>{t('admin.th_stock')}</th>
                    <th style={{ textAlign: 'right' }}>{t('admin.th_actions')}</th>
                  </tr>
                </thead>
                <tbody>
                  {products.map(p => (
                    <tr key={p.id}>
                      <td>
                        {p.imageUrl ? (
                          <img src={p.imageUrl} alt={p.name} className="product-table-thumb" />
                        ) : (
                          <div className="product-table-thumb placeholder"><Tag size={16} /></div>
                        )}
                      </td>
                      <td><strong>{p.code}</strong></td>
                      <td>{p.name}</td>
                      <td style={{ color: '#60a5fa', fontWeight: 600 }}>{formatCurrency(p.price)}</td>
                      <td>
                        <span className="barcode-badge">
                          <Barcode size={12} style={{ marginRight: '4px' }} />
                          {p.barcode}
                        </span>
                      </td>
                      <td>{p.stockQuantity}</td>
                      <td style={{ textAlign: 'right' }}>
                        <div className="row-actions">
                          <button className="edit-action" onClick={() => {
                            setEditingProduct(p);
                            setProductImageBase64(p.imageUrl);
                            setIsProductFormOpen(true);
                          }}><Edit size={14} /></button>
                          <button className="delete-action" onClick={() => handleDeleteProduct(p.id)}><Trash2 size={14} /></button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {/* ==========================================
          USER MANAGEMENT TAB
          ========================================== */}
      {activeSubTab === 'users' && (
        <div className="admin-panel-content glass-container">
          <div className="panel-header">
            <h3>{t('admin.users_header', { defaultValue: 'Quản Trị Nhân Sự Hệ Thống' })}</h3>
            <button className="panel-action-btn" onClick={() => {
              setEditingUser({ userName: '', fullName: '', email: '', password: '', role: 'Staff', branchId: '' });
              setIsUserFormOpen(true);
            }}>
              <UserPlus size={16} />
              {t('admin.add_user', { defaultValue: 'Thêm nhân viên mới' })}
            </button>
          </div>

          {usersLoading ? (
            <div className="spinner-wrapper"><div className="spinner"></div></div>
          ) : (
            <div className="admin-table-container">
              <table className="admin-table">
                <thead>
                  <tr>
                    <th>{t('admin.th_username')}</th>
                    <th>{t('admin.th_fullname')}</th>
                    <th>{t('admin.th_email')}</th>
                    <th>{t('admin.th_role')}</th>
                    <th>{t('layout.branch_label').replace(':', '')}</th>
                    <th>{t('admin.th_status')}</th>
                    <th style={{ textAlign: 'right' }}>{t('admin.th_actions')}</th>
                  </tr>
                </thead>
                <tbody>
                  {users.map(u => (
                    <tr key={u.id}>
                      <td><strong>{u.userName}</strong></td>
                      <td>{u.fullName}</td>
                      <td>{u.email}</td>
                      <td>
                        <span className={`role-badge ${u.roles.includes('Admin') ? 'admin' : 'staff'}`}>
                          {u.roles.join(', ')}
                        </span>
                      </td>
                      <td>{u.branchName || t('layout.hq_label')}</td>
                      <td>
                        <span className={`status-badge-inline ${u.isActive ? 'active' : 'inactive'}`}>
                          {u.isActive ? t('common.status_active') : t('common.status_locked')}
                        </span>
                      </td>
                      <td style={{ textAlign: 'right' }}>
                        <div className="row-actions">
                          <button className="edit-action" onClick={() => {
                            setEditingUser({
                              id: u.id,
                              userName: u.userName,
                              fullName: u.fullName,
                              email: u.email,
                              branchId: u.branchId || '',
                              role: u.roles[0] || 'Staff'
                            });
                            setIsUserFormOpen(true);
                          }}><Edit size={14} /></button>
                          <button className="delete-action" onClick={() => handleDeleteUser(u.id)}><Trash2 size={14} /></button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {/* ==========================================
          BRANCH MANAGEMENT TAB
          ========================================== */}
      {activeSubTab === 'branches' && (
        <div className="admin-panel-content glass-container">
          <div className="panel-header">
            <h3>{t('admin.branches_header', { defaultValue: 'Danh Sách Chi Nhánh Cửa Hàng' })}</h3>
            <button className="panel-action-btn" onClick={() => {
              setEditingBranch({ name: '', address: '', phone: '', isActive: true });
              setIsBranchFormOpen(true);
            }}>
              <Plus size={16} />
              {t('admin.add_branch', { defaultValue: 'Thêm chi nhánh mới' })}
            </button>
          </div>

          {branchesLoading ? (
            <div className="spinner-wrapper"><div className="spinner"></div></div>
          ) : (
            <div className="admin-table-container">
              <table className="admin-table">
                <thead>
                  <tr>
                    <th>{t('admin.th_branch_name')}</th>
                    <th>{t('admin.th_address')}</th>
                    <th>{t('admin.th_phone')}</th>
                    <th>{t('admin.th_status')}</th>
                    <th style={{ textAlign: 'right' }}>{t('admin.th_actions')}</th>
                  </tr>
                </thead>
                <tbody>
                  {branches.map(b => (
                    <tr key={b.id}>
                      <td><strong>{b.name}</strong></td>
                      <td>{b.address || 'N/A'}</td>
                      <td>{b.phone || 'N/A'}</td>
                      <td>
                        <span className={`status-badge-inline ${b.isActive ? 'active' : 'inactive'}`}>
                          {b.isActive ? t('common.status_active') : t('common.status_inactive')}
                        </span>
                      </td>
                      <td style={{ textAlign: 'right' }}>
                        <div className="row-actions">
                          <button className="edit-action" onClick={() => {
                            setEditingBranch(b);
                            setIsBranchFormOpen(true);
                          }}><Edit size={14} /></button>
                          <button className="delete-action" onClick={() => handleDeleteBranch(b.id)}><Trash2 size={14} /></button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}

      {/* ==========================================
          PRODUCT FORM MODAL
          ========================================== */}
      {isProductFormOpen && editingProduct && (
        <div className="modal-overlay">
          <div className="modal-content glass-container">
            <div className="modal-header">
              <h3>{editingProduct.id ? 'Chỉnh Sửa Sản Phẩm' : 'Thêm Sản Phẩm Mới'}</h3>
              <button className="modal-close" onClick={() => {
                setIsProductFormOpen(false);
                setEditingProduct(null);
                setProductImageBase64(undefined);
              }}><X size={20} /></button>
            </div>
            <form onSubmit={handleSaveProduct} className="modal-form">
              <div className="form-group">
                <label>Mã sản phẩm</label>
                <input 
                  type="text" 
                  value={editingProduct.id ? (editingProduct.code || '') : '(Hệ thống tự sinh)'}
                  disabled
                  style={{ opacity: 0.6, cursor: 'not-allowed', background: 'var(--card-inset)' }}
                />
              </div>

              <div className="form-group">
                <label>Tên sản phẩm *</label>
                <input 
                  type="text" 
                  value={editingProduct.name || ''}
                  onChange={(e) => setEditingProduct({ ...editingProduct, name: e.target.value })}
                  placeholder="Nhập tên sản phẩm..."
                  required
                />
              </div>

              <div className="form-row">
                <div className="form-group">
                  <label>Đơn giá bán (VND) *</label>
                  <input 
                    type="number" 
                    value={editingProduct.price || 0}
                    onChange={(e) => setEditingProduct({ ...editingProduct, price: Number(e.target.value) })}
                    placeholder="Đơn giá bán"
                    required
                  />
                </div>
                <div className="form-group">
                  <label>Đơn giá nhập (VND) *</label>
                  <input 
                    type="number" 
                    value={editingProduct.importPrice || 0}
                    onChange={(e) => setEditingProduct({ ...editingProduct, importPrice: Number(e.target.value) })}
                    placeholder="Đơn giá nhập"
                    required
                  />
                </div>
              </div>

              <div className="form-group">
                <label>Mã vạch (Barcode)</label>
                <div style={{ display: 'flex', gap: '8px' }}>
                  <input 
                    type="text" 
                    value={editingProduct.barcode || ''}
                    onChange={(e) => setEditingProduct({ ...editingProduct, barcode: e.target.value })}
                    placeholder="Mã vạch EAN/UPC..."
                    style={{ flex: 1 }}
                  />
                  <button type="button" className="barcode-gen-btn" onClick={handleGenerateBarcode}>
                    <Barcode size={16} />
                    Tạo mã ngẫu nhiên
                  </button>
                </div>
              </div>

              <div className="form-group">
                <label>Hình ảnh sản phẩm</label>
                <div className="img-upload-field">
                  {productImageBase64 ? (
                    <img src={productImageBase64} alt="Preview" className="img-upload-preview" />
                  ) : (
                    <div className="img-upload-placeholder">Chưa tải ảnh lên</div>
                  )}
                  <label className="img-upload-btn">
                    <Upload size={14} style={{ marginRight: '6px' }} />
                    Chọn ảnh
                    <input type="file" accept="image/*" onChange={handleProductImageUpload} style={{ display: 'none' }} />
                  </label>
                </div>
              </div>

              <button type="submit" className="form-submit-btn">
                <Save size={16} style={{ marginRight: '6px' }} />
                Lưu sản phẩm
              </button>
            </form>
          </div>
        </div>
      )}

      {/* ==========================================
          USER FORM MODAL
          ========================================== */}
      {isUserFormOpen && editingUser && (
        <div className="modal-overlay">
          <div className="modal-content glass-container">
            <div className="modal-header">
              <h3>{editingUser.id ? t('admin.edit_user_header') : t('admin.create_user_header')}</h3>
              <button className="modal-close" onClick={() => setIsUserFormOpen(false)}><X size={20} /></button>
            </div>
            <form onSubmit={handleSaveUser} className="modal-form">
              <div className="form-group">
                <label>{t('admin.label_username')}</label>
                <input 
                  type="text" 
                  value={editingUser.userName}
                  onChange={(e) => setEditingUser({ ...editingUser, userName: e.target.value })}
                  placeholder="username..."
                  disabled={!!editingUser.id}
                  required
                />
              </div>

              <div className="form-group">
                <label>{t('admin.label_fullname')}</label>
                <input 
                  type="text" 
                  value={editingUser.fullName}
                  onChange={(e) => setEditingUser({ ...editingUser, fullName: e.target.value })}
                  placeholder={t('crm.placeholder_fullname')}
                  required
                />
              </div>

              <div className="form-group">
                <label>{t('admin.label_email')}</label>
                <input 
                  type="email" 
                  value={editingUser.email}
                  onChange={(e) => setEditingUser({ ...editingUser, email: e.target.value })}
                  placeholder="example@salescrm.com..."
                  required
                />
              </div>

              {!editingUser.id && (
                <div className="form-group">
                  <label>{t('admin.label_password')}</label>
                  <input 
                    type="password" 
                    value={editingUser.password}
                    onChange={(e) => setEditingUser({ ...editingUser, password: e.target.value })}
                    placeholder={t('admin.placeholder_password')}
                    required={!editingUser.id}
                  />
                </div>
              )}

              <div className="form-row">
                <div className="form-group">
                  <label>{t('admin.label_system_role')}</label>
                  <select 
                    value={editingUser.role} 
                    onChange={(e) => setEditingUser({ ...editingUser, role: e.target.value })}
                    className="admin-select"
                  >
                    <option value="Staff">{t('admin.option_role_staff')}</option>
                    <option value="Admin">{t('admin.option_role_admin')}</option>
                  </select>
                </div>

                <div className="form-group">
                  <label>{t('admin.label_active_branch')}</label>
                  <select 
                    value={editingUser.branchId} 
                    onChange={(e) => setEditingUser({ ...editingUser, branchId: e.target.value })}
                    className="admin-select"
                  >
                    <option value="">{t('admin.option_hq')}</option>
                    {branches.map(b => (
                      <option key={b.id} value={b.id}>{b.name}</option>
                    ))}
                  </select>
                </div>
              </div>

              <button type="submit" className="form-submit-btn">
                <Save size={16} style={{ marginRight: '6px' }} />
                {t('admin.btn_save_user')}
              </button>
            </form>
          </div>
        </div>
      )}

      {/* ==========================================
          BRANCH FORM MODAL
          ========================================== */}
      {isBranchFormOpen && editingBranch && (
        <div className="modal-overlay">
          <div className="modal-content glass-container">
            <div className="modal-header">
              <h3>{editingBranch.id ? t('admin.edit_branch_header') : t('admin.create_branch_header')}</h3>
              <button className="modal-close" onClick={() => {
                setIsBranchFormOpen(false);
                setEditingBranch(null);
              }}><X size={20} /></button>
            </div>
            <form onSubmit={handleSaveBranch} className="modal-form">
              <div className="form-group">
                <label>{t('admin.label_branch_name')}</label>
                <input 
                  type="text" 
                  value={editingBranch.name || ''}
                  onChange={(e) => setEditingBranch({ ...editingBranch, name: e.target.value })}
                  placeholder={t('admin.placeholder_branch_name')}
                  required
                />
              </div>

              <div className="form-group">
                <label>{t('admin.label_address')}</label>
                <input 
                  type="text" 
                  value={editingBranch.address || ''}
                  onChange={(e) => setEditingBranch({ ...editingBranch, address: e.target.value })}
                  placeholder={t('admin.placeholder_address')}
                />
              </div>

              <div className="form-group">
                <label>{t('admin.label_phone')}</label>
                <input 
                  type="text" 
                  value={editingBranch.phone || ''}
                  onChange={(e) => setEditingBranch({ ...editingBranch, phone: e.target.value })}
                  placeholder={t('admin.placeholder_phone')}
                />
              </div>

              {editingBranch.id && (
                <div className="form-group" style={{ display: 'flex', gap: '8px', alignItems: 'center' }}>
                  <input 
                    type="checkbox" 
                    id="branchActiveCheck"
                    checked={editingBranch.isActive ?? true}
                    onChange={(e) => setEditingBranch({ ...editingBranch, isActive: e.target.checked })}
                    style={{ cursor: 'pointer', width: '16px', height: '16px' }}
                  />
                  <label htmlFor="branchActiveCheck" style={{ cursor: 'pointer', fontSize: '0.85rem' }}>{t('admin.label_active')}</label>
                </div>
              )}

              <button type="submit" className="form-submit-btn">
                <Save size={16} style={{ marginRight: '6px' }} />
                {t('admin.btn_save_branch')}
              </button>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};
