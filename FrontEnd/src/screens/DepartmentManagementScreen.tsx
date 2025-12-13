import React, { useState, useEffect } from 'react';
import { View, Text, StyleSheet, ScrollView, TouchableOpacity, Alert, ActivityIndicator, Modal, FlatList } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { Colors, Spacing, FontSize, BorderRadius } from '../constants/theme';
import { ArrowLeft, Building, Users, Edit2, X, Check, Search } from 'lucide-react-native';
import { NativeStackScreenProps } from '@react-navigation/native-stack';
import { RootStackParamList } from '../types/types';
import { departmentService, Department, UserOption } from '../services/departmentService';

type Props = NativeStackScreenProps<RootStackParamList, 'DepartmentManagement'>;

export default function DepartmentManagementScreen({ navigation }: Props) {
  const [departments, setDepartments] = useState<Department[]>([]);
  const [users, setUsers] = useState<UserOption[]>([]);
  const [loading, setLoading] = useState(true);
  
  // Edit modal
  const [showEditModal, setShowEditModal] = useState(false);
  const [selectedDept, setSelectedDept] = useState<Department | null>(null);
  const [selectedManagerId, setSelectedManagerId] = useState<string | undefined>();
  const [selectedDeputyId, setSelectedDeputyId] = useState<string | undefined>();
  const [saving, setSaving] = useState(false);

  // User picker
  const [showUserPicker, setShowUserPicker] = useState(false);
  const [pickerType, setPickerType] = useState<'manager' | 'deputy'>('manager');

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    setLoading(true);
    try {
      const [depts, usersResult] = await Promise.all([
        departmentService.getDepartments(),
        departmentService.getAvailableLeaders(),
      ]);
      setDepartments(depts);
      if (usersResult.isSuccessed && usersResult.resultObj) {
        setUsers(usersResult.resultObj);
      }
    } catch (error) {
      console.error('Load data error:', error);
    }
    setLoading(false);
  };

  const openEditModal = (dept: Department) => {
    setSelectedDept(dept);
    setSelectedManagerId(dept.managerId);
    setSelectedDeputyId(dept.deputyId);
    setShowEditModal(true);
  };

  const openUserPicker = (type: 'manager' | 'deputy') => {
    setPickerType(type);
    setShowUserPicker(true);
  };

  const selectUser = (user: UserOption) => {
    if (pickerType === 'manager') {
      if (user.id === selectedDeputyId) {
        Alert.alert('Lỗi', 'Không thể chọn cùng người làm cả Trưởng và Phó phòng');
        return;
      }
      setSelectedManagerId(user.id);
    } else {
      if (user.id === selectedManagerId) {
        Alert.alert('Lỗi', 'Không thể chọn cùng người làm cả Trưởng và Phó phòng');
        return;
      }
      setSelectedDeputyId(user.id);
    }
    setShowUserPicker(false);
  };

  const handleSave = async () => {
    if (!selectedDept) return;

    setSaving(true);
    const result = await departmentService.updateLeadership(selectedDept.id, {
      managerId: selectedManagerId,
      deputyId: selectedDeputyId,
    });

    if (result.isSuccessed) {
      Alert.alert('Thành công', 'Đã cập nhật lãnh đạo phòng ban');
      setShowEditModal(false);
      loadData();
    } else {
      Alert.alert('Lỗi', result.message || 'Không thể cập nhật');
    }
    setSaving(false);
  };

  const getSelectedUserName = (userId?: string) => {
    if (!userId) return 'Chưa chọn';
    const user = users.find(u => u.id === userId);
    return user?.fullName || 'Chưa chọn';
  };

  return (
    <SafeAreaView style={styles.container}>
      {/* Header */}
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()} style={styles.backBtn}>
          <ArrowLeft size={24} color={Colors.text} />
        </TouchableOpacity>
        <Text style={styles.headerTitle}>Quản lý Phòng ban</Text>
        <View style={{ width: 32 }} />
      </View>

      {/* Content */}
      <ScrollView contentContainerStyle={styles.content}>
        {loading ? (
          <ActivityIndicator size="large" color={Colors.primary} style={{ marginTop: 40 }} />
        ) : departments.length === 0 ? (
          <View style={styles.emptyState}>
            <Building size={48} color={Colors.textLight} />
            <Text style={styles.emptyText}>Chưa có phòng ban nào</Text>
          </View>
        ) : (
          departments.map((dept) => (
            <View key={dept.id} style={styles.deptCard}>
              <View style={styles.cardHeader}>
                <View style={styles.deptIcon}>
                  <Building size={20} color={Colors.primary} />
                </View>
                <View style={styles.deptInfo}>
                  <Text style={styles.deptName}>{dept.name}</Text>
                  <Text style={styles.deptCount}>
                    <Users size={12} color={Colors.textSecondary} /> {dept.userCount} thành viên
                  </Text>
                </View>
                <TouchableOpacity style={styles.editBtn} onPress={() => openEditModal(dept)}>
                  <Edit2 size={18} color={Colors.primary} />
                </TouchableOpacity>
              </View>

              <View style={styles.leadershipInfo}>
                <View style={styles.leaderRow}>
                  <Text style={styles.leaderLabel}>Trưởng phòng:</Text>
                  <Text style={styles.leaderName}>{dept.managerName || 'Chưa có'}</Text>
                </View>
                <View style={styles.leaderRow}>
                  <Text style={styles.leaderLabel}>Phó phòng:</Text>
                  <Text style={styles.leaderName}>{dept.deputyName || 'Chưa có'}</Text>
                </View>
              </View>
            </View>
          ))
        )}
      </ScrollView>

      {/* Edit Modal */}
      <Modal visible={showEditModal} transparent animationType="fade">
        <View style={styles.modalOverlay}>
          <View style={styles.modalContent}>
            <View style={styles.modalHeader}>
              <Text style={styles.modalTitle}>Thay đổi lãnh đạo</Text>
              <TouchableOpacity onPress={() => setShowEditModal(false)}>
                <X size={24} color={Colors.text} />
              </TouchableOpacity>
            </View>

            <Text style={styles.deptNameModal}>{selectedDept?.name}</Text>

            {/* Manager Picker */}
            <Text style={styles.pickerLabel}>Trưởng phòng</Text>
            <TouchableOpacity style={styles.pickerBtn} onPress={() => openUserPicker('manager')}>
              <Text style={styles.pickerBtnText}>{getSelectedUserName(selectedManagerId)}</Text>
            </TouchableOpacity>

            {/* Deputy Picker */}
            <Text style={styles.pickerLabel}>Phó phòng</Text>
            <TouchableOpacity style={styles.pickerBtn} onPress={() => openUserPicker('deputy')}>
              <Text style={styles.pickerBtnText}>{getSelectedUserName(selectedDeputyId)}</Text>
            </TouchableOpacity>

            <TouchableOpacity
              style={[styles.saveBtn, saving && styles.saveBtnDisabled]}
              onPress={handleSave}
              disabled={saving}
            >
              {saving ? (
                <ActivityIndicator color="white" />
              ) : (
                <>
                  <Check size={18} color="white" />
                  <Text style={styles.saveBtnText}>Lưu thay đổi</Text>
                </>
              )}
            </TouchableOpacity>
          </View>
        </View>
      </Modal>

      {/* User Picker Modal */}
      <Modal visible={showUserPicker} transparent animationType="slide">
        <View style={styles.pickerOverlay}>
          <View style={styles.pickerContent}>
            <View style={styles.pickerHeader}>
              <Text style={styles.pickerTitle}>
                Chọn {pickerType === 'manager' ? 'Trưởng phòng' : 'Phó phòng'}
              </Text>
              <TouchableOpacity onPress={() => setShowUserPicker(false)}>
                <X size={24} color={Colors.text} />
              </TouchableOpacity>
            </View>

            <FlatList
              data={users}
              keyExtractor={(item) => item.id}
              renderItem={({ item }) => (
                <TouchableOpacity style={styles.userItem} onPress={() => selectUser(item)}>
                  <View style={styles.userAvatar}>
                    <Text style={styles.userAvatarText}>{item.fullName.charAt(0)}</Text>
                  </View>
                  <View style={styles.userInfo}>
                    <Text style={styles.userFullName}>{item.fullName}</Text>
                    <Text style={styles.userEmail}>{item.email}</Text>
                    {item.currentDepartment && (
                      <Text style={styles.userDept}>Hiện tại: {item.currentDepartment}</Text>
                    )}
                  </View>
                </TouchableOpacity>
              )}
            />
          </View>
        </View>
      </Modal>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: { flex: 1, backgroundColor: Colors.background },
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: Spacing.lg,
    backgroundColor: 'white',
    borderBottomWidth: 1,
    borderBottomColor: Colors.border,
  },
  backBtn: { padding: Spacing.xs },
  headerTitle: { fontSize: FontSize.lg, fontWeight: 'bold', color: Colors.text },
  content: { padding: Spacing.lg },

  // Empty
  emptyState: { alignItems: 'center', marginTop: 60 },
  emptyText: { color: Colors.textSecondary, fontSize: FontSize.md, marginTop: Spacing.md },

  // Card
  deptCard: {
    backgroundColor: 'white',
    borderRadius: BorderRadius.md,
    padding: Spacing.md,
    marginBottom: Spacing.md,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 4,
    elevation: 2,
  },
  cardHeader: { flexDirection: 'row', alignItems: 'center', marginBottom: Spacing.md },
  deptIcon: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: Colors.background,
    alignItems: 'center',
    justifyContent: 'center',
    marginRight: Spacing.md,
  },
  deptInfo: { flex: 1 },
  deptName: { fontSize: FontSize.md, fontWeight: 'bold', color: Colors.text },
  deptCount: { fontSize: FontSize.xs, color: Colors.textSecondary, marginTop: 2 },
  editBtn: { padding: Spacing.sm },
  leadershipInfo: { borderTopWidth: 1, borderTopColor: Colors.border, paddingTop: Spacing.sm },
  leaderRow: { flexDirection: 'row', marginBottom: 4 },
  leaderLabel: { fontSize: FontSize.sm, color: Colors.textSecondary, width: 100 },
  leaderName: { fontSize: FontSize.sm, color: Colors.text, fontWeight: '500' },

  // Modal
  modalOverlay: {
    flex: 1,
    backgroundColor: 'rgba(0,0,0,0.5)',
    justifyContent: 'center',
    alignItems: 'center',
  },
  modalContent: {
    backgroundColor: 'white',
    borderRadius: BorderRadius.md,
    padding: Spacing.lg,
    width: '85%',
  },
  modalHeader: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center', marginBottom: Spacing.md },
  modalTitle: { fontSize: FontSize.lg, fontWeight: 'bold', color: Colors.text },
  deptNameModal: { fontSize: FontSize.md, color: Colors.primary, fontWeight: '600', marginBottom: Spacing.lg },
  pickerLabel: { fontSize: FontSize.sm, fontWeight: '600', color: Colors.textSecondary, marginBottom: Spacing.xs },
  pickerBtn: {
    backgroundColor: Colors.background,
    padding: Spacing.md,
    borderRadius: BorderRadius.sm,
    borderWidth: 1,
    borderColor: Colors.border,
    marginBottom: Spacing.md,
  },
  pickerBtnText: { fontSize: FontSize.md, color: Colors.text },
  saveBtn: {
    flexDirection: 'row',
    backgroundColor: Colors.primary,
    padding: Spacing.md,
    borderRadius: BorderRadius.sm,
    alignItems: 'center',
    justifyContent: 'center',
    gap: Spacing.xs,
    marginTop: Spacing.md,
  },
  saveBtnDisabled: { backgroundColor: Colors.primaryLight },
  saveBtnText: { color: 'white', fontWeight: 'bold', fontSize: FontSize.md },

  // User Picker
  pickerOverlay: {
    flex: 1,
    backgroundColor: 'rgba(0,0,0,0.5)',
    justifyContent: 'flex-end',
  },
  pickerContent: {
    backgroundColor: 'white',
    borderTopLeftRadius: BorderRadius.lg,
    borderTopRightRadius: BorderRadius.lg,
    maxHeight: '70%',
  },
  pickerHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: Spacing.lg,
    borderBottomWidth: 1,
    borderBottomColor: Colors.border,
  },
  pickerTitle: { fontSize: FontSize.lg, fontWeight: 'bold', color: Colors.text },
  userItem: {
    flexDirection: 'row',
    padding: Spacing.md,
    borderBottomWidth: 1,
    borderBottomColor: Colors.border,
    alignItems: 'center',
  },
  userAvatar: {
    width: 40,
    height: 40,
    borderRadius: 20,
    backgroundColor: Colors.primaryLight,
    alignItems: 'center',
    justifyContent: 'center',
    marginRight: Spacing.md,
  },
  userAvatarText: { color: 'white', fontWeight: 'bold', fontSize: FontSize.md },
  userInfo: { flex: 1 },
  userFullName: { fontSize: FontSize.md, fontWeight: '600', color: Colors.text },
  userEmail: { fontSize: FontSize.sm, color: Colors.textSecondary },
  userDept: { fontSize: FontSize.xs, color: Colors.primary, marginTop: 2 },
});
