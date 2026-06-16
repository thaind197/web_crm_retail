import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { useAuthStore } from '../../store/useAuthStore';
import { 
  TrendingUp, 
  Activity,
  Truck,
  Package,
  AlertCircle,
  Eye,
  Search
} from 'lucide-react';
import { 
  LineChart, 
  Line, 
  XAxis, 
  YAxis, 
  CartesianGrid, 
  Tooltip, 
  ResponsiveContainer 
} from 'recharts';
import axios from 'axios';
import { API_BASE_URL } from '../../config';
import './Dashboard.css';

interface DashboardData {
  totalRevenue: number;
  totalOrders: number;
  averageOrderValue: number;
  totalBranches?: number;
  totalProducts?: number;
  dailyRevenue: { dateStr: string; revenue: number; orderCount: number }[];
  topProducts: { productName: string; quantitySold: number; revenueGenerated: number }[];
  revenueByBranch?: { branchName: string; revenue: number; orderCount: number }[];
}

export const Dashboard: React.FC = () => {
  const { t } = useTranslation();
  const { token, roles, branchId } = useAuthStore();
  const isAdmin = roles.includes('Admin');
  const [data, setData] = useState<DashboardData | null>(null);
  const [loading, setLoading] = useState(true);
  const [recentOrders, setRecentOrders] = useState<any[]>([]);
  const [searchTerm, setSearchTerm] = useState('');

  // Default dates (last 30 days)
  const endDate = new Date().toISOString().split('T')[0];
  const startDate = new Date(Date.now() - 30 * 24 * 60 * 60 * 1000).toISOString().split('T')[0];

  useEffect(() => {
    const fetchDashboard = async () => {
      setLoading(true);
      try {
        const endpoint = isAdmin 
          ? `${API_BASE_URL}/api/reports/system?startDate=${startDate}&endDate=${endDate}`
          : `${API_BASE_URL}/api/reports/branch?branchId=${branchId || ''}&startDate=${startDate}&endDate=${endDate}`;

        const res = await axios.get(endpoint, {
          headers: { Authorization: `Bearer ${token}` }
        });

        if (res.data.isSuccess && res.data.data) {
          const raw = res.data.data;
          setData({
            totalRevenue: raw.totalRevenue,
            totalOrders: raw.totalOrders,
            averageOrderValue: raw.averageOrderValue || (raw.totalOrders === 0 ? 0 : raw.totalRevenue / raw.totalOrders),
            totalBranches: raw.totalBranches,
            totalProducts: raw.totalProducts,
            dailyRevenue: raw.dailyRevenue || raw.systemDailyRevenue || [],
            topProducts: raw.topProducts || raw.globalTopProducts || [],
            revenueByBranch: raw.revenueByBranch
          });
        }

        // Fetch recent orders
        try {
          const ordersRes = await axios.get(`${API_BASE_URL}/api/orders?pageSize=5`, {
            headers: { Authorization: `Bearer ${token}` }
          });
          if (ordersRes.data.isSuccess && ordersRes.data.data?.items) {
            setRecentOrders(ordersRes.data.data.items);
          } else {
            throw new Error("No items in response");
          }
        } catch (orderErr) {
          console.warn("Failed to fetch recent orders.", orderErr);
          setRecentOrders([]);
        }
      } catch (err) {
        console.warn("Failed to fetch live reports.", err);
        setData(null);
        setRecentOrders([]);
      } finally {
        setLoading(false);
      }
    };

    fetchDashboard();
  }, [isAdmin, branchId, token, startDate, endDate]);

  const formatCurrency = (val: number) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val);
  };

  if (loading || !data) {
    return (
      <div className="loading-state">
        <div className="spinner"></div>
        <p>{t('common.loading')}</p>
      </div>
    );
  }

  const filteredOrders = recentOrders.filter(o => 
    o.orderCode.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (o.customer?.name || t('dashboard.walkin_customer', { defaultValue: 'Khách vãng lai' })).toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div className="dashboard-container">
      <div className="dashboard-header-row">
        <div>
          <h1 className="dashboard-title glow-text">{t('dashboard.title')}</h1>
          <p className="dashboard-subtitle">{t('dashboard.last_update')}</p>
        </div>
        <div className="live-badge">
          <Activity className="pulse-icon" size={16} />
          <span>LIVE UPDATES</span>
        </div>
      </div>

      {/* KPI Cards Row */}
      <div className="kpi-grid">
        <div className="kpi-card glass-container">
          <div className="kpi-card-header">
            <span className="kpi-label">{t('dashboard.kpi_revenue')}</span>
            <TrendingUp size={20} style={{ color: '#10b981' }} />
          </div>
          <span className="kpi-value">{formatCurrency(data.totalRevenue)}</span>
          <span className="kpi-growth">{t('dashboard.growth_revenue')}</span>
        </div>

        <div className="kpi-card glass-container">
          <div className="kpi-card-header">
            <span className="kpi-label">{t('dashboard.kpi_orders')}</span>
            <Truck size={20} style={{ color: '#3b82f6' }} />
          </div>
          <span className="kpi-value">{data.totalOrders}</span>
          <span className="kpi-growth" style={{ color: 'var(--text-secondary)' }}>{t('dashboard.growth_orders')}</span>
        </div>

        <div className="kpi-card glass-container">
          <div className="kpi-card-header">
            <span className="kpi-label">{t('dashboard.kpi_stock')}</span>
            <Package size={20} style={{ color: '#f59e0b' }} />
          </div>
          <span className="kpi-value">850.000.000 ₫</span>
          <span className="kpi-growth" style={{ color: '#f59e0b' }}>{t('dashboard.growth_stock')}</span>
        </div>

        <div className="kpi-card glass-container">
          <div className="kpi-card-header">
            <span className="kpi-label">{t('dashboard.kpi_errors')}</span>
            <AlertCircle size={20} style={{ color: '#f43f5e' }} />
          </div>
          <span className="kpi-value">24</span>
          <span className="kpi-growth" style={{ color: '#f43f5e', fontWeight: 600 }}>{t('dashboard.growth_errors')}</span>
        </div>
      </div>

      {/* Chart and Activity Grid */}
      <div className="dashboard-mid-grid">
        {/* Daily Revenue Line Chart */}
        <div className="chart-card glass-container">
          <div className="chart-header-row">
            <h3 className="chart-title">{t('dashboard.chart_title')}</h3>
            <a href="/reports" className="view-report-link" onClick={(e) => e.preventDefault()}>{t('dashboard.chart_detail')}</a>
          </div>
          <div className="chart-wrapper">
            <ResponsiveContainer width="100%" height={300}>
              <LineChart data={data.dailyRevenue} margin={{ top: 10, right: 10, left: 10, bottom: 0 }}>
                <defs>
                  <linearGradient id="colorRev" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="5%" stopColor="#8b5cf6" stopOpacity={0.4}/>
                    <stop offset="95%" stopColor="#8b5cf6" stopOpacity={0}/>
                  </linearGradient>
                </defs>
                <CartesianGrid strokeDasharray="3 3" stroke="var(--card-border)" />
                <XAxis dataKey="dateStr" stroke="var(--text-secondary)" fontSize={12} tickLine={false} />
                <YAxis stroke="var(--text-secondary)" fontSize={12} tickLine={false} tickFormatter={(v) => `${v/1000000}M`} />
                <Tooltip 
                  contentStyle={{ backgroundColor: 'var(--input-bg)', borderColor: 'var(--card-border)', borderRadius: '8px' }}
                  labelStyle={{ color: 'var(--text-primary)' }}
                  formatter={(value: any) => [formatCurrency(Number(value)), t('dashboard.revenue')]} 
                />
                <Line 
                  type="monotone" 
                  dataKey="revenue" 
                  stroke="#a855f7" 
                  strokeWidth={3} 
                  dot={{ r: 4, stroke: '#c084fc', strokeWidth: 2 }}
                  activeDot={{ r: 8 }}
                  fillOpacity={1}
                  fill="url(#colorRev)"
                />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Warehouse Activity */}
        <div className="activity-card glass-container">
          <h3 className="chart-title">{t('dashboard.activity_title')}</h3>
          <div className="activity-list">
            <div className="activity-item">
              <div className="activity-icon-badge inbound-badge">
                <Truck size={16} />
              </div>
              <div className="activity-info">
                <span className="activity-title">SKU-9921 Inbound</span>
                <span className="activity-desc">{t('dashboard.activity_inbound_desc')}</span>
                <span className="activity-time">{t('dashboard.time_10m')}</span>
              </div>
            </div>

            <div className="activity-item">
              <div className="activity-icon-badge outbound-badge">
                <Truck size={16} style={{ transform: 'scaleX(-1)' }} />
              </div>
              <div className="activity-info">
                <span className="activity-title">SKU-4412 Outbound</span>
                <span className="activity-desc">{t('dashboard.activity_transfer_desc')}</span>
                <span className="activity-time">{t('dashboard.time_1h')}</span>
              </div>
            </div>

            <div className="activity-item">
              <div className="activity-icon-badge audit-badge">
                <Package size={16} />
              </div>
              <div className="activity-info">
                <span className="activity-title">{t('dashboard.activity_audit')}</span>
                <span className="activity-desc">{t('dashboard.activity_audit_desc')}</span>
                <span className="activity-time">{t('dashboard.time_3h')}</span>
              </div>
            </div>
          </div>
          <button className="view-all-activity-btn" onClick={(e) => e.preventDefault()}>{t('dashboard.activity_all')}</button>
        </div>
      </div>

      {/* Recent Transactions Table */}
      <div className="recent-transactions-card glass-container">
        <div className="transactions-header-row">
          <h3 className="chart-title">{t('dashboard.recent_orders')}</h3>
          <div className="search-wrapper">
            <Search size={16} className="search-icon" />
            <input 
              type="text" 
              placeholder={t('dashboard.search_orders')} 
              className="transaction-search"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
        </div>

        <div className="table-responsive">
          <table className="transactions-table">
            <thead>
              <tr>
                <th>{t('dashboard.th_code')}</th>
                <th>{t('dashboard.th_customer')}</th>
                <th>{t('dashboard.th_amount')}</th>
                <th>{t('dashboard.th_status')}</th>
                <th style={{ textAlign: 'right' }}>{t('dashboard.th_actions')}</th>
              </tr>
            </thead>
            <tbody>
              {filteredOrders.length === 0 ? (
                <tr>
                  <td colSpan={5} style={{ textAlign: 'center', padding: '24px', color: 'var(--text-muted)' }}>
                    {t('dashboard.empty_orders')}
                  </td>
                </tr>
              ) : (
                filteredOrders.map(o => (
                  <tr key={o.id}>
                    <td className="t-code">{o.orderCode}</td>
                    <td className="t-customer">{o.customer?.name || t('dashboard.walkin_customer')}</td>
                    <td className="t-amount">{formatCurrency(o.finalAmount || o.totalAmount)}</td>
                    <td>
                      <span className={`t-status-badge status-${o.status}`}>
                        {o.status === 2 ? t('dashboard.status_completed') : o.status === 1 ? t('dashboard.status_processing') : t('dashboard.status_pending')}
                      </span>
                    </td>
                    <td style={{ textAlign: 'right' }}>
                      <button className="t-action-btn" title={t('common.view_details')} onClick={(e) => e.preventDefault()}>
                        <Eye size={16} />
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};
