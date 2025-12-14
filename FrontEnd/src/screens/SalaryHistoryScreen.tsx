import React, { useState, useEffect, useCallback } from 'react';
import {
  View,
  Text,
  FlatList,
  StyleSheet,
  TouchableOpacity,
  ActivityIndicator,
  RefreshControl,
  Alert,
  Modal,
  ScrollView,
  TextInput,
  KeyboardAvoidingView,
  Platform,
} from 'react-native';
import { salaryService, SalaryVm, ComplaintVm } from '../services/salaryService';
import { departmentService, Department } from '../services/departmentService';
import { userService, User } from '../services/userService';
import { useNavigation } from '@react-navigation/native';
import { Calculator, CheckCircle } from 'lucide-react-native';
import { Colors } from '../constants/theme';

const months = [
  { label: 'Tháng 1', value: 1 }, { label: 'Tháng 2', value: 2 },
  { label: 'Tháng 3', value: 3 }, { label: 'Tháng 4', value: 4 },
  { label: 'Tháng 5', value: 5 }, { label: 'Tháng 6', value: 6 },
  { label: 'Tháng 7', value: 7 }, { label: 'Tháng 8', value: 8 },
  { label: 'Tháng 9', value: 9 }, { label: 'Tháng 10', value: 10 },
  { label: 'Tháng 11', value: 11 }, { label: 'Tháng 12', value: 12 },
];

interface SalaryRowItem {
  userId: string;
  employeeId: string; // May be null if user is not employee? But system assumes Users are Employees mostly.
  employeeName: string;
  departmentName: string;
  salary?: SalaryVm;
  isCalculated: boolean;
}

const SalaryHistoryScreen: React.FC = () => {
  const navigation = useNavigation();
  const [data, setData] = useState<SalaryRowItem[]>([]);
  const [departments, setDepartments] = useState<Department[]>([]);
  const [complaints, setComplaints] = useState<ComplaintVm[]>([]);
  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);

  const [selectedDept, setSelectedDept] = useState<string>('');
  const [selectedMonth, setSelectedMonth] = useState<number>(new Date().getMonth() + 1);
  const [selectedYear, setSelectedYear] = useState<number>(new Date().getFullYear());

  const [showDeptModal, setShowDeptModal] = useState(false);
  const [showMonthModal, setShowMonthModal] = useState(false);
  const [showYearModal, setShowYearModal] = useState(false);
  const [showComplaintModal, setShowComplaintModal] = useState(false);

  // Calculate Modal State
  const [showCalcModal, setShowCalcModal] = useState(false);
  const [calcEmployeeId, setCalcEmployeeId] = useState<string>('');
  const [calcPreview, setCalcPreview] = useState<SalaryVm | null>(null);
  const [calcBonus, setCalcBonus] = useState('');
  const [calcAllowance, setCalcAllowance] = useState('');
  // const [calcDeductions, setCalcDeductions] = useState(''); // Removed per user request
  const [calcNote, setCalcNote] = useState('');
  const [calculating, setCalculating] = useState(false);

  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const pageSize = 15;

  const loadDepartments = useCallback(async () => {
    const result = await departmentService.getDepartments();
    if (result) setDepartments(result);
  }, []);

  const loadComplaints = useCallback(async () => {
    const result = await salaryService.getAllComplaints();
    if (result.isSuccessed && result.resultObj) setComplaints(result.resultObj);
  }, []);

  const loadData = useCallback(async (page = 1) => {
    try {
      setLoading(true);
      // 1. Get Users (Paginated)
      // Note: userService.getUsers currently doesn't support department filter in frontend params, 
      // but we will implement client-side filtering or assume backend update later.
      // For now, fetching all users might be heavy, so we rely on pagination.
      // Ideally we need userService.getUsers(keyword, page, size, departmentId).
      const usersRes = await userService.getUsers('', page, pageSize);

      // 2. Get Salaries for this month (Fetch ALL to match, or fetch paged? 
      // Existing getAllSalary supports filtering by Dept.
      // If we paginate users, we should ideally fetch salaries for THESE users.
      // But for simplicity, we fetch salaries ensuring we cover the visible users.
      // Actually, let's fetch salaries dynamically where salary.employeeId matches.
      const salariesRes = await salaryService.getAllSalary(
        selectedMonth,
        selectedYear,
        selectedDept || undefined,
        1,
        1000 // Fetch large batch to ensure we find matches
      );

      if (usersRes && salariesRes.isSuccessed && salariesRes.resultObj) {
        const users = usersRes.items;
        const salaries = salariesRes.resultObj.items;

        // Filter users by Dept if selected (Client side since API might not support it yet)
        // Wait, filtering client side destroys pagination accuracy. 
        // We will assume for now we list all users.

        const mergedData: SalaryRowItem[] = users.map(u => {
          // Direct name matching as fallback, but ideally use employeeId if available from User object (now available!)
          // But wait, getAllSalary returns SalaryVm which has EmployeeId. User now has EmployeeId.
          // BEST MATCH: Match by EmployeeId.

          let salary = salaries.find(s => s.employeeId === u.employeeId);
          if (!salary && !u.employeeId) {
            // Fallback to name match if employeeId is missing (e.g. legacy data)
            salary = salaries.find(s => s.employeeName === u.fullName);
          }

          return {
            userId: u.id,
            employeeId: u.employeeId || salary?.employeeId || '',
            employeeName: u.fullName,
            departmentName: u.departmentName || 'Unknown',
            salary: salary,
            isCalculated: !!salary
          };
        });

        setData(mergedData);
        setTotalPages(usersRes.totalPages); // Use Users pagination
        setCurrentPage(usersRes.pageIndex);
      }
    } catch (error) {
      Alert.alert('Lỗi', 'Không thể tải dữ liệu');
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, [selectedDept, selectedMonth, selectedYear]);

  useEffect(() => { loadDepartments(); loadComplaints(); }, [loadDepartments, loadComplaints]);
  useEffect(() => { loadData(1); }, [selectedDept, selectedMonth, selectedYear, loadData]);

  const handleRefresh = () => { setRefreshing(true); loadData(1); loadComplaints(); };

  const getDeptName = () => !selectedDept ? 'Tất cả' : departments.find(d => d.id === selectedDept)?.name || 'Tất cả';
  const getMonthLabel = () => months.find(m => m.value === selectedMonth)?.label || '';

  const formatMoney = (amount: number) => new Intl.NumberFormat('vi-VN').format(amount) + 'đ';

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Draft': return '#9E9E9E';
      case 'Pending': return Colors.warning;
      case 'Approved': return Colors.primary;
      case 'Paid': return Colors.success;
      case 'Confirmed': return Colors.success;
      case 'Complaining': return Colors.error;
      case 'Rejected': return Colors.error;
      case 'Cancelled': return Colors.textSecondary;
      default: return Colors.textSecondary;
    }
  };

  const handleOpenCalculate = async (employeeId: string) => {
    setCalcEmployeeId(employeeId);
    setCalcBonus('');
    setCalcAllowance('');
    // setCalcDeductions('');
    setCalcNote('');
    setCalcPreview(null);
    setShowCalcModal(true);

    // Fetch initial preview (default 0 for inputs)
    setCalculating(true);
    try {
      const result = await salaryService.calculateSalaryPreview({
        employeeId,
        month: selectedMonth,
        year: selectedYear
      });
      if (result.isSuccessed && result.resultObj) {
        setCalcPreview(result.resultObj);
      }
    } catch {
      // ignore
    } finally {
      setCalculating(false);
    }
  };

  const handleRecalculatePreview = async () => {
    if (!calcEmployeeId) return;
    setCalculating(true);
    try {
      const result = await salaryService.calculateSalaryPreview({
        employeeId: calcEmployeeId,
        month: selectedMonth,
        year: selectedYear,
        bonus: Number(calcBonus) || 0,
        allowance: Number(calcAllowance) || 0,
        deductions: 0, // Number(calcDeductions) || 0,
        note: calcNote
      });
      if (result.isSuccessed && result.resultObj) {
        setCalcPreview(result.resultObj);
      }
    } catch {
      Alert.alert('Lỗi', 'Không thể tính toán preview');
    } finally {
      setCalculating(false);
    }
  };

  const handleSaveCalculation = async () => {
    if (!calcEmployeeId) return;
    Alert.alert('Xác nhận', 'Bạn có chắc muốn lưu bảng lương này?', [
      { text: 'Hủy', style: 'cancel' },
      {
        text: 'Lưu',
        onPress: async () => {
          try {
            const result = await salaryService.calculateSalary({
              employeeId: calcEmployeeId,
              month: selectedMonth,
              year: selectedYear,
              bonus: Number(calcBonus) || 0,
              allowance: Number(calcAllowance) || 0,
              deductions: 0, // Number(calcDeductions) || 0,
              note: calcNote
            });
            if (result.isSuccessed) {
              Alert.alert('Thành công', 'Đã lưu bảng lương');
              setShowCalcModal(false);
              loadData(currentPage);
            } else {
              Alert.alert('Lỗi', result.message || 'Không thể lưu');
            }
          } catch (error) {
            Alert.alert('Lỗi', 'Có lỗi xảy ra');
          }
        }
      }
    ]);
  };

  const handlePay = async (id: string) => {
    Alert.alert('Xác nhận', 'Bạn có chắc muốn thanh toán lương này?', [
      { text: 'Hủy', style: 'cancel' },
      {
        text: 'Thanh toán',
        onPress: async () => {
          try {
            const result = await salaryService.paySalary(id);
            if (result.isSuccessed) {
              Alert.alert('Thành công', 'Đã thanh toán lương');
              loadData(currentPage);
            } else {
              Alert.alert('Lỗi', result.message || 'Không thể thanh toán');
            }
          } catch (error) {
            Alert.alert('Lỗi', 'Có lỗi xảy ra');
          }
        },
      },
    ]);
  };

  const renderItem = ({ item }: { item: SalaryRowItem }) => {
    const s = item.salary;

    if (!s) {
      // Not Calculated
      return (
        <View style={styles.card}>
          <View style={styles.cardHeader}>
            <View>
              <Text style={styles.employeeName}>{item.employeeName}</Text>
              <Text style={styles.deptName}>{item.departmentName}</Text>
            </View>
            <View style={[styles.statusBadge, { backgroundColor: '#999' }]}>
              <Text style={styles.statusText}>Chưa tính</Text>
            </View>
          </View>

          <TouchableOpacity
            style={[styles.payBtn, { backgroundColor: Colors.primary, flexDirection: 'row', alignItems: 'center', justifyContent: 'center' }]}
            onPress={() => handleOpenCalculate(item.employeeId)}
          >
            <Calculator size={18} color="white" style={{ marginRight: 8 }} />
            <Text style={styles.payBtnText}>Tính lương</Text>
          </TouchableOpacity>
        </View>
      );
    }

    return (
      <View style={styles.card}>
        <View style={styles.cardHeader}>
          <View>
            <Text style={styles.employeeName}>{item.employeeName}</Text>
            <Text style={styles.deptName}>{item.departmentName}</Text>
          </View>
          <View style={[styles.statusBadge, { backgroundColor: getStatusColor(s.status) }]}>
            <Text style={styles.statusText}>{s.statusName}</Text>
          </View>
        </View>
        <View style={styles.salaryInfo}>
          <View style={styles.salaryRow}>
            <Text style={styles.label}>Lương cơ bản:</Text>
            <Text style={styles.value}>{formatMoney(s.baseSalary)}</Text>
          </View>
          <View style={styles.salaryRow}>
            <Text style={styles.label}>Phụ cấp + OT:</Text>
            <Text style={styles.valueGreen}>{formatMoney(s.allowance + s.overtimePay + s.bonus)}</Text>
          </View>
          <View style={styles.salaryRow}>
            <Text style={styles.label}>Khấu trừ:</Text>
            <Text style={styles.valueRed}>-{formatMoney(s.deductions + s.insurance + s.tax)}</Text>
          </View>
          <View style={[styles.salaryRow, styles.netRow]}>
            <Text style={styles.netLabel}>Thực lĩnh:</Text>
            <Text style={styles.netValue}>{formatMoney(s.netSalary)}</Text>
          </View>
        </View>
        {s.status === 'Approved' && (
          <TouchableOpacity style={styles.payBtn} onPress={() => handlePay(s.id)}>
            <Text style={styles.payBtnText}>💰 Thanh toán</Text>
          </TouchableOpacity>
        )}
        {(s.status === 'Confirmed' || s.status === 'Paid') && (
          // Just show status, actions done by Employee
          <View style={{ marginTop: 8, flexDirection: 'row', alignItems: 'center' }}>
            {s.status === 'Confirmed' && <CheckCircle size={14} color={Colors.success} style={{ marginRight: 4 }} />}
            <Text style={{ fontSize: 12, color: getStatusColor(s.status) }}>
              {s.status === 'Confirmed' ? 'Nhân viên đã xác nhận' : s.status === 'Paid' ? 'Đã thanh toán (Chờ xác nhận)' : ''}
            </Text>
          </View>
        )}
      </View>
    );
  };

  const renderComplaintItem = ({ item }: { item: ComplaintVm }) => (
    <View style={styles.complaintCard}>
      <View style={styles.cardHeader}>
        <Text style={styles.employeeName}>{item.employeeName}</Text>
        <Text style={styles.complaintDate}>{item.month}/{item.year}</Text>
      </View>
      <Text style={styles.complaintType}>{item.complaintTypeName}</Text>
      <Text style={styles.complaintContent}>{item.content}</Text>
      <View style={[styles.statusBadge, { backgroundColor: item.status === 1 ? '#4CAF50' : item.status === 2 ? '#F44336' : '#FFA000', marginTop: 8, alignSelf: 'flex-start' }]}>
        <Text style={styles.statusText}>{item.statusName}</Text>
      </View>
    </View>
  );

  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()} style={styles.backBtn}>
          <Text style={styles.backText}>← Quay lại</Text>
        </TouchableOpacity>
        <Text style={styles.title}>Lịch sử Lương</Text>
        <TouchableOpacity style={styles.complaintBtn} onPress={() => setShowComplaintModal(true)}>
          <Text style={styles.complaintBtnText}>📝 ({complaints.length})</Text>
        </TouchableOpacity>
      </View>

      <View style={styles.filterRow}>
        <View style={styles.filterItem}>
          <Text style={styles.filterLabel}>Phòng ban:</Text>
          <TouchableOpacity style={styles.dropdown} onPress={() => setShowDeptModal(true)}>
            <Text style={styles.dropdownText}>{getDeptName()}</Text>
            <Text style={styles.dropdownArrow}>▼</Text>
          </TouchableOpacity>
        </View>
      </View>
      <View style={styles.filterRow}>
        <View style={styles.filterItem}>
          <Text style={styles.filterLabel}>Tháng:</Text>
          <TouchableOpacity style={styles.dropdown} onPress={() => setShowMonthModal(true)}>
            <Text style={styles.dropdownText}>{getMonthLabel()}</Text>
            <Text style={styles.dropdownArrow}>▼</Text>
          </TouchableOpacity>
        </View>
        <View style={styles.filterItem}>
          <Text style={styles.filterLabel}>Năm:</Text>
          <TouchableOpacity style={styles.dropdown} onPress={() => setShowYearModal(true)}>
            <Text style={styles.dropdownText}>{selectedYear}</Text>
            <Text style={styles.dropdownArrow}>▼</Text>
          </TouchableOpacity>
        </View>
      </View>

      {loading && !refreshing ? (
        <ActivityIndicator size="large" color="#4CAF50" style={styles.loader} />
      ) : (
        <FlatList
          data={data}
          keyExtractor={item => item.salary?.id || item.userId}
          renderItem={renderItem}
          contentContainerStyle={styles.listContent}
          refreshControl={<RefreshControl refreshing={refreshing} onRefresh={handleRefresh} />}
          ListEmptyComponent={<Text style={styles.emptyText}>Không có dữ liệu</Text>}
        />
      )}

      {totalPages > 1 && (
        <View style={styles.pagination}>
          <TouchableOpacity style={[styles.pageBtn, currentPage <= 1 && styles.pageBtnDisabled]} onPress={() => loadData(currentPage - 1)} disabled={currentPage <= 1}>
            <Text style={styles.pageBtnText}>« Trước</Text>
          </TouchableOpacity>
          <Text style={styles.pageInfo}>Trang {currentPage}/{totalPages}</Text>
          <TouchableOpacity style={[styles.pageBtn, currentPage >= totalPages && styles.pageBtnDisabled]} onPress={() => loadData(currentPage + 1)} disabled={currentPage >= totalPages}>
            <Text style={styles.pageBtnText}>Sau »</Text>
          </TouchableOpacity>
        </View>
      )}

      {/* Dept Modal */}
      <Modal visible={showDeptModal} transparent animationType="slide">
        <View style={styles.modalOverlay}>
          <View style={styles.modalContent}>
            <Text style={styles.modalTitle}>Chọn phòng ban</Text>
            <ScrollView style={styles.modalScroll}>
              <TouchableOpacity style={styles.modalOption} onPress={() => { setSelectedDept(''); setShowDeptModal(false); }}>
                <Text style={[styles.modalOptionText, !selectedDept && styles.modalOptionSelected]}>Tất cả</Text>
              </TouchableOpacity>
              {departments.map(d => (
                <TouchableOpacity key={d.id} style={styles.modalOption} onPress={() => { setSelectedDept(d.id); setShowDeptModal(false); }}>
                  <Text style={[styles.modalOptionText, selectedDept === d.id && styles.modalOptionSelected]}>{d.name}</Text>
                </TouchableOpacity>
              ))}
            </ScrollView>
            <TouchableOpacity style={styles.modalClose} onPress={() => setShowDeptModal(false)}><Text style={styles.modalCloseText}>Đóng</Text></TouchableOpacity>
          </View>
        </View>
      </Modal>

      {/* Month Modal */}
      <Modal visible={showMonthModal} transparent animationType="slide">
        <View style={styles.modalOverlay}>
          <View style={styles.modalContent}>
            <Text style={styles.modalTitle}>Chọn tháng</Text>
            <ScrollView style={styles.modalScroll}>
              {months.map(m => (
                <TouchableOpacity key={m.value} style={styles.modalOption} onPress={() => { setSelectedMonth(m.value); setShowMonthModal(false); }}>
                  <Text style={[styles.modalOptionText, selectedMonth === m.value && styles.modalOptionSelected]}>{m.label}</Text>
                </TouchableOpacity>
              ))}
            </ScrollView>
            <TouchableOpacity style={styles.modalClose} onPress={() => setShowMonthModal(false)}><Text style={styles.modalCloseText}>Đóng</Text></TouchableOpacity>
          </View>
        </View>
      </Modal>

      {/* Year Modal */}
      <Modal visible={showYearModal} transparent animationType="slide">
        <View style={styles.modalOverlay}>
          <View style={styles.modalContent}>
            <Text style={styles.modalTitle}>Chọn năm</Text>
            <ScrollView style={styles.modalScroll}>
              {[2023, 2024, 2025].map(y => (
                <TouchableOpacity key={y} style={styles.modalOption} onPress={() => { setSelectedYear(y); setShowYearModal(false); }}>
                  <Text style={[styles.modalOptionText, selectedYear === y && styles.modalOptionSelected]}>{y}</Text>
                </TouchableOpacity>
              ))}
            </ScrollView>
            <TouchableOpacity style={styles.modalClose} onPress={() => setShowYearModal(false)}><Text style={styles.modalCloseText}>Đóng</Text></TouchableOpacity>
          </View>
        </View>
      </Modal>

      {/* Complaint Modal */}
      <Modal visible={showComplaintModal} animationType="slide" transparent>
        <View style={styles.modalOverlay}>
          <View style={styles.modalContentLarge}>
            <View style={styles.modalHeader}>
              <Text style={styles.modalTitle}>Khiếu nại lương</Text>
              <TouchableOpacity onPress={() => setShowComplaintModal(false)}><Text style={styles.closeBtn}>✕</Text></TouchableOpacity>
            </View>
            <FlatList
              data={complaints}
              keyExtractor={item => item.id}
              renderItem={renderComplaintItem}
              ListEmptyComponent={<Text style={styles.emptyText}>Không có khiếu nại</Text>}
            />
          </View>
        </View>
      </Modal>

      {/* Calculate Modal */}
      <Modal visible={showCalcModal} animationType="slide" transparent>
        <KeyboardAvoidingView
          behavior={Platform.OS === "ios" ? "padding" : "height"}
          style={styles.modalOverlay}
        >
          <View style={styles.modalContentLarge}>
            <View style={styles.modalHeader}>
              <Text style={styles.modalTitle}>Tính lương {selectedMonth}/{selectedYear}</Text>
              <TouchableOpacity onPress={() => setShowCalcModal(false)}><Text style={styles.closeBtn}>✕</Text></TouchableOpacity>
            </View>

            <ScrollView style={{ flex: 1 }}>
              {calculating && <ActivityIndicator size="small" color={Colors.primary} style={{ marginBottom: 10 }} />}

              {calcPreview && (
                <View style={styles.previewContainer}>
                  <Text style={styles.previewHeader}>{calcPreview.employeeName}</Text>
                  <View style={styles.previewRow}><Text>Ngày công:</Text><Text>{calcPreview.actualWorkDays}/{calcPreview.workDays}</Text></View>
                  <View style={styles.previewRow}><Text>Đi muộn/Về sớm:</Text><Text>{calcPreview.lateDays + calcPreview.earlyLeaveDays}</Text></View>
                  <View style={styles.previewRow}><Text>Lương CB:</Text><Text>{formatMoney(calcPreview.baseSalary)}</Text></View>
                  <View style={styles.previewRow}><Text>OT Pay:</Text><Text>{formatMoney(calcPreview.overtimePay)}</Text></View>
                </View>
              )}

              <Text style={styles.inputLabel}>Thưởng (Bonus):</Text>
              <TextInput
                style={styles.input}
                keyboardType="numeric"
                value={calcBonus}
                onChangeText={setCalcBonus}
                placeholder="0"
              />

              <Text style={styles.inputLabel}>Phụ cấp (Allowance):</Text>
              <TextInput
                style={styles.input}
                keyboardType="numeric"
                value={calcAllowance}
                onChangeText={setCalcAllowance}
                placeholder="0"
              />

              {/* 
              <Text style={styles.inputLabel}>Khấu trừ thêm (Deductions):</Text>
              <TextInput
                style={styles.input}
                keyboardType="numeric"
                value={calcDeductions}
                onChangeText={setCalcDeductions}
                placeholder="0"
              /> 
              */}

              <Text style={styles.inputLabel}>Ghi chú:</Text>
              <TextInput
                style={styles.input}
                value={calcNote}
                onChangeText={setCalcNote}
                placeholder="Ghi chú (nếu có)"
              />

              <TouchableOpacity style={styles.previewBtn} onPress={handleRecalculatePreview}>
                <Text style={styles.previewBtnText}>Cập nhật xem trước</Text>
              </TouchableOpacity>

              {calcPreview && (
                <View style={[styles.previewContainer, { backgroundColor: '#E8F5E9', borderColor: '#4CAF50' }]}>
                  <View style={styles.previewRow}><Text style={{ fontWeight: 'bold' }}>Thực lĩnh (Net):</Text><Text style={{ fontWeight: 'bold', color: '#4CAF50', fontSize: 16 }}>{formatMoney(calcPreview.netSalary)}</Text></View>
                </View>
              )}

            </ScrollView>

            <TouchableOpacity style={styles.saveBtn} onPress={handleSaveCalculation}>
              <Text style={styles.saveBtnText}>Lưu & Tính lương</Text>
            </TouchableOpacity>
          </View>
        </KeyboardAvoidingView>
      </Modal>
    </View >
  );
};

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: '#f5f5f5' },
  header: { flexDirection: 'row', alignItems: 'center', backgroundColor: '#4CAF50', padding: 16, paddingTop: 48 },
  backBtn: { marginRight: 12 },
  backText: { color: '#fff', fontSize: 16 },
  title: { color: '#fff', fontSize: 20, fontWeight: 'bold', flex: 1 },
  complaintBtn: { backgroundColor: 'rgba(255,255,255,0.2)', padding: 8, borderRadius: 8 },
  complaintBtnText: { color: '#fff', fontSize: 12 },
  filterRow: { flexDirection: 'row', paddingHorizontal: 12, paddingVertical: 6, backgroundColor: '#fff' },
  filterItem: { flex: 1, marginHorizontal: 4 },
  filterLabel: { fontSize: 12, color: '#666', marginBottom: 4 },
  dropdown: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', backgroundColor: '#f0f0f0', borderRadius: 8, padding: 10 },
  dropdownText: { fontSize: 14, color: '#333' },
  dropdownArrow: { fontSize: 10, color: '#666' },
  listContent: { padding: 12 },
  loader: { marginTop: 40 },
  card: { backgroundColor: '#fff', borderRadius: 12, padding: 16, marginBottom: 12, elevation: 2 },
  cardHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: 12 },
  employeeName: { fontSize: 16, fontWeight: '600', color: '#333' },
  deptName: { fontSize: 12, color: '#888' },
  statusBadge: { paddingHorizontal: 10, paddingVertical: 4, borderRadius: 12 },
  statusText: { fontSize: 12, fontWeight: '600', color: '#fff' },
  salaryInfo: { borderTopWidth: 1, borderTopColor: '#eee', paddingTop: 12 },
  salaryRow: { flexDirection: 'row', justifyContent: 'space-between', marginBottom: 6 },
  label: { color: '#666', fontSize: 14 },
  value: { color: '#333', fontSize: 14 },
  valueGreen: { color: '#4CAF50', fontSize: 14 },
  valueRed: { color: '#F44336', fontSize: 14 },
  netRow: { borderTopWidth: 1, borderTopColor: '#eee', paddingTop: 8, marginTop: 4 },
  netLabel: { fontSize: 16, fontWeight: '600', color: '#333' },
  netValue: { fontSize: 18, fontWeight: 'bold', color: '#4CAF50' },
  payBtn: { backgroundColor: '#4CAF50', padding: 12, borderRadius: 8, marginTop: 12, alignItems: 'center' },
  payBtnText: { color: '#fff', fontWeight: '600' },
  emptyText: { textAlign: 'center', color: '#999', marginTop: 40 },
  pagination: { flexDirection: 'row', justifyContent: 'center', alignItems: 'center', padding: 12, backgroundColor: '#fff', borderTopWidth: 1, borderTopColor: '#eee' },
  pageBtn: { backgroundColor: '#4CAF50', paddingHorizontal: 16, paddingVertical: 8, borderRadius: 8, marginHorizontal: 8 },
  pageBtnDisabled: { backgroundColor: '#ccc' },
  pageBtnText: { color: '#fff', fontWeight: '600' },
  pageInfo: { color: '#666' },
  modalOverlay: { flex: 1, backgroundColor: 'rgba(0,0,0,0.5)', justifyContent: 'flex-end' },
  modalContent: { backgroundColor: '#fff', borderTopLeftRadius: 20, borderTopRightRadius: 20, maxHeight: '50%', padding: 16 },
  modalContentLarge: { backgroundColor: '#fff', borderTopLeftRadius: 20, borderTopRightRadius: 20, maxHeight: '80%', padding: 16 },
  modalHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: 16 },
  modalTitle: { fontSize: 18, fontWeight: 'bold', textAlign: 'center' },
  closeBtn: { fontSize: 24, color: '#666' },
  modalScroll: { maxHeight: 300 },
  modalOption: { paddingVertical: 12, borderBottomWidth: 1, borderBottomColor: '#eee' },
  modalOptionText: { fontSize: 16, color: '#333' },
  modalOptionSelected: { color: '#4CAF50', fontWeight: '600' },
  modalClose: { marginTop: 12, padding: 12, backgroundColor: '#f0f0f0', borderRadius: 8, alignItems: 'center' },
  modalCloseText: { fontSize: 16, color: '#666' },
  complaintCard: { backgroundColor: '#f9f9f9', borderRadius: 8, padding: 12, marginBottom: 12 },
  complaintDate: { fontSize: 12, color: '#888' },
  complaintType: { fontSize: 14, fontWeight: '600', color: '#333', marginTop: 4 },
  complaintContent: { fontSize: 13, color: '#666', marginTop: 4 },
  input: { borderWidth: 1, borderColor: '#ddd', borderRadius: 8, padding: 10, marginBottom: 12, fontSize: 16 },
  inputLabel: { fontSize: 14, fontWeight: '600', color: '#333', marginBottom: 4 },
  previewContainer: { backgroundColor: '#f9f9f9', padding: 12, borderRadius: 8, marginBottom: 16, borderWidth: 1, borderColor: '#eee' },
  previewHeader: { fontSize: 16, fontWeight: 'bold', marginBottom: 8, color: '#333' },
  previewRow: { flexDirection: 'row', justifyContent: 'space-between', marginBottom: 4 },
  previewBtn: { backgroundColor: '#2196F3', padding: 12, borderRadius: 8, alignItems: 'center', marginBottom: 16 },
  previewBtnText: { color: '#fff', fontWeight: 'bold' },
  saveBtn: { backgroundColor: '#4CAF50', padding: 14, borderRadius: 8, alignItems: 'center' },
  saveBtnText: { color: '#fff', fontWeight: 'bold', fontSize: 16 },
});

export default SalaryHistoryScreen;
