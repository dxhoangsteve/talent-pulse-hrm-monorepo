import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  StyleSheet,
  TouchableOpacity,
  FlatList,
  ActivityIndicator,
  TextInput,
  Alert,
  Modal,
  ScrollView,
} from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { Colors, Spacing, FontSize, BorderRadius } from '../constants/theme';
import { userService, User } from '../services/userService';
import { departmentService, Department } from '../services/departmentService';
import { Search, Plus, Trash2, Edit, Key, ArrowLeft } from 'lucide-react-native';
import { useNavigation } from '@react-navigation/native';

export default function UserManagementScreen() {
  const navigation = useNavigation();
  const [users, setUsers] = useState<User[]>([]);
  const [departments, setDepartments] = useState<Department[]>([]);
  const [loading, setLoading] = useState(false);
  const [keyword, setKeyword] = useState('');
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);

  // Modal States
  const [modalVisible, setModalVisible] = useState(false);
  const [passwordModalVisible, setPasswordModalVisible] = useState(false);
  const [editingUser, setEditingUser] = useState<User | null>(null);
  
  // Form States
  const [formData, setFormData] = useState({
    userName: '',
    email: '',
    password: '',
    confirmPassword: '',
    phone: '',
    baseSalary: '',
    departmentId: '',
  });
  
  const [newPassword, setNewPassword] = useState('');

  useEffect(() => {
    loadDepartments();
  }, []);

  useEffect(() => {
    loadUsers();
  }, [page, keyword]);

  const loadUsers = async () => {
    setLoading(true);
    try {
      const result = await userService.getUsers(keyword, page, 10);
      setUsers(result.items);
      setTotalPages(result.totalPages);
    } catch (error) {
      Alert.alert('Lỗi', 'Không thể tải danh sách nhân viên');
    } finally {
      setLoading(false);
    }
  };

  const loadDepartments = async () => {
    try {
      const depts = await departmentService.getDepartments();
      setDepartments(depts);
    } catch (error) {
      console.log('Could not load departments:', error);
    }
  };

  const handleDelete = (user: User) => {
    if (user.roles?.includes('SuperAdmin') || user.userName.toLowerCase() === 'superadmin') {
        Alert.alert('Cảnh báo', 'Không thể xóa tài khoản SuperAdmin!');
        return;
    }
    Alert.alert(
      'Xác nhận xóa',
      `Bạn có chắc muốn xóa nhân viên ${user.userName}?`,
      [
        { text: 'Hủy', style: 'cancel' },
        {
          text: 'Xóa',
          style: 'destructive',
          onPress: async () => {
            try {
              await userService.deleteUser(user.id);
              loadUsers();
              Alert.alert('Thành công', 'Đã xóa nhân viên');
            } catch (error: any) {
              Alert.alert('Lỗi', error.message || 'Xóa thất bại');
            }
          },
        },
      ]
    );
  };

  const handleSaveUser = async () => {
    try {
      if (editingUser) {
        // Update
        await userService.updateUser(editingUser.id, {
            email: formData.email,
            phone: formData.phone
        });
        Alert.alert('Thành công', 'Cập nhật thành công');
      } else {
        // Create
        if (formData.password !== formData.confirmPassword) {
            Alert.alert('Lỗi', 'Mật khẩu xác nhận không khớp');
            return;
        }
        if (!formData.baseSalary || parseFloat(formData.baseSalary) < 0) {
            Alert.alert('Lỗi', 'Vui lòng nhập lương cơ bản hợp lệ');
            return;
        }
        await userService.createUser({
            userName: formData.userName,
            password: formData.password,
            confirmPassword: formData.confirmPassword,
            email: formData.email,
            firstName: formData.userName,
            lastName: '',
            phoneNumber: formData.phone,
            linkFB: 'https://facebook.com',
            baseSalary: parseFloat(formData.baseSalary),
            departmentId: formData.departmentId || null,
        });
        Alert.alert('Thành công', 'Tạo nhân viên thành công');
      }
      setModalVisible(false);
      resetForm();
      loadUsers();
    } catch (error: any) {
        console.error("Save User Error:", error);
        const errorMessage = error?.message || (typeof error === 'object' ? JSON.stringify(error) : 'Có lỗi xảy ra');
        Alert.alert('Lỗi', errorMessage);
    }
  };

  const handleChangePassword = async () => {
      if (!editingUser) return;
      if (newPassword.length < 6) {
          Alert.alert('Lỗi', 'Mật khẩu phải từ 6 ký tự');
          return;
      }
      try {
          await userService.setPassword(editingUser.id, newPassword);
          Alert.alert('Thành công', 'Đã đổi mật khẩu');
          setPasswordModalVisible(false);
          setNewPassword('');
          setEditingUser(null);
      } catch (error: any) {
          Alert.alert('Lỗi', error.message);
      }
  }

  const openAddModal = () => {
      setEditingUser(null);
      resetForm();
      setModalVisible(true);
  }

  const openEditModal = (user: User) => {
      setEditingUser(user);
      setFormData({
          userName: user.userName,
          email: user.email || '',
          phone: user.phoneNumber || '',
          password: '',
          confirmPassword: '',
          baseSalary: '',
          departmentId: '',
      });
      setModalVisible(true);
  }

  const openPasswordModal = (user: User) => {
      setEditingUser(user);
      setNewPassword('');
      setPasswordModalVisible(true);
  }

  const resetForm = () => {
      setFormData({
        userName: '',
        email: '',
        password: '',
        confirmPassword: '',
        phone: '',
        baseSalary: '',
        departmentId: '',
      });
  }

  const renderItem = ({ item }: { item: User }) => (
    <View style={styles.userCard}>
      <View style={styles.userInfo}>
        <Text style={styles.userName}>{item.userName}</Text>
        <Text style={styles.userEmail}>{item.email}</Text>
        <Text style={styles.userPhone}>{item.phoneNumber}</Text>
      </View>
      <View style={styles.actions}>
        <TouchableOpacity onPress={() => openEditModal(item)} style={styles.iconBtn}>
            <Edit size={20} color={Colors.primary} />
        </TouchableOpacity>
        <TouchableOpacity onPress={() => openPasswordModal(item)} style={styles.iconBtn}>
            <Key size={20} color={Colors.warning} />
        </TouchableOpacity>
        <TouchableOpacity onPress={() => handleDelete(item)} style={styles.iconBtn}>
            <Trash2 size={20} color={Colors.error} />
        </TouchableOpacity>
      </View>
    </View>
  );

  return (
    <SafeAreaView style={styles.container}>
      <View style={styles.header}>
        <TouchableOpacity onPress={() => navigation.goBack()} style={styles.backButton}>
            <ArrowLeft color={Colors.text} size={24} />
        </TouchableOpacity>
        <Text style={styles.title}>Quản lý nhân viên</Text>
        <TouchableOpacity onPress={openAddModal} style={styles.addButton}>
            <Plus color="white" size={24} />
        </TouchableOpacity>
      </View>

      <View style={styles.searchContainer}>
        <Search color={Colors.textSecondary} size={20} />
        <TextInput 
            style={styles.searchInput}
            placeholder="Tìm kiếm..."
            value={keyword}
            onChangeText={setKeyword}
        />
      </View>

      {loading ? (
          <ActivityIndicator size="large" color={Colors.primary} style={{marginTop: 20}} />
      ) : (
          <FlatList
            data={users}
            renderItem={renderItem}
            keyExtractor={item => item.id}
            contentContainerStyle={styles.list}
          />
      )}

      {/* Add/Edit Modal */}
      <Modal visible={modalVisible} animationType="slide" transparent>
          <View style={styles.modalOverlay}>
              <View style={styles.modalContent}>
                  <Text style={styles.modalTitle}>{editingUser ? 'Sửa nhân viên' : 'Thêm nhân viên'}</Text>
                  
                  {!editingUser && (
                    <>
                        <TextInput 
                            style={styles.input} 
                            placeholder="Username" 
                            value={formData.userName}
                            onChangeText={t => setFormData({...formData, userName: t})}
                        />
                         <TextInput 
                            style={styles.input} 
                            placeholder="Mật khẩu" 
                            secureTextEntry
                            value={formData.password}
                            onChangeText={t => setFormData({...formData, password: t})}
                        />
                         <TextInput 
                            style={styles.input} 
                            placeholder="Xác nhận mật khẩu" 
                            secureTextEntry
                            value={formData.confirmPassword}
                            onChangeText={t => setFormData({...formData, confirmPassword: t})}
                        />
                        <TextInput 
                            style={styles.input} 
                            placeholder="Lương cơ bản *" 
                            value={formData.baseSalary}
                            onChangeText={t => setFormData({...formData, baseSalary: t})}
                            keyboardType="numeric"
                        />
                    </>
                  )}
                  
                  <TextInput 
                        style={styles.input} 
                        placeholder="Email" 
                        value={formData.email}
                        onChangeText={t => setFormData({...formData, email: t})}
                        autoCapitalize="none"
                    />
                   <TextInput 
                        style={styles.input} 
                        placeholder="Số điện thoại" 
                        value={formData.phone}
                        onChangeText={t => setFormData({...formData, phone: t})}
                        keyboardType="phone-pad"
                    />

                   {/* Department Picker */}
                   {!editingUser && (
                     <View style={styles.pickerContainer}>
                       <Text style={styles.pickerLabel}>Phòng ban (tùy chọn):</Text>
                       <ScrollView horizontal showsHorizontalScrollIndicator={false} style={styles.deptScroll}>
                         <TouchableOpacity 
                           style={[styles.deptChip, !formData.departmentId && styles.deptChipActive]}
                           onPress={() => setFormData({...formData, departmentId: ''})}
                         >
                           <Text style={[styles.deptChipText, !formData.departmentId && styles.deptChipTextActive]}>Không có</Text>
                         </TouchableOpacity>
                         {departments.map(dept => (
                           <TouchableOpacity 
                             key={dept.id}
                             style={[styles.deptChip, formData.departmentId === dept.id && styles.deptChipActive]}
                             onPress={() => setFormData({...formData, departmentId: dept.id})}
                           >
                             <Text style={[styles.deptChipText, formData.departmentId === dept.id && styles.deptChipTextActive]}>{dept.name}</Text>
                           </TouchableOpacity>
                         ))}
                       </ScrollView>
                     </View>
                   )}

                  <View style={styles.modalActions}>
                      <TouchableOpacity onPress={() => setModalVisible(false)} style={[styles.btn, styles.btnCancel]}>
                          <Text style={styles.btnText}>Hủy</Text>
                      </TouchableOpacity>
                      <TouchableOpacity onPress={handleSaveUser} style={[styles.btn, styles.btnSave]}>
                          <Text style={{...styles.btnText, color: 'white'}}>Lưu</Text>
                      </TouchableOpacity>
                  </View>
              </View>
          </View>
      </Modal>

      {/* Password Modal */}
      <Modal visible={passwordModalVisible} animationType="slide" transparent>
          <View style={styles.modalOverlay}>
              <View style={styles.modalContent}>
                  <Text style={styles.modalTitle}>Đổi mật khẩu cho {editingUser?.userName}</Text>
                  <TextInput 
                        style={styles.input} 
                        placeholder="Mật khẩu mới" 
                        secureTextEntry
                        value={newPassword}
                        onChangeText={setNewPassword}
                    />
                  <View style={styles.modalActions}>
                      <TouchableOpacity onPress={() => setPasswordModalVisible(false)} style={[styles.btn, styles.btnCancel]}>
                          <Text style={styles.btnText}>Hủy</Text>
                      </TouchableOpacity>
                      <TouchableOpacity onPress={handleChangePassword} style={[styles.btn, styles.btnSave]}>
                          <Text style={{...styles.btnText, color: 'white'}}>Lưu</Text>
                      </TouchableOpacity>
                  </View>
              </View>
          </View>
      </Modal>

    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  header: {
      flexDirection: 'row',
      justifyContent: 'space-between',
      alignItems: 'center',
      padding: Spacing.lg,
  },
  title: {
      fontSize: FontSize.xl,
      fontWeight: 'bold',
      color: Colors.text,
  },
  backButton: {
      padding: Spacing.xs,
  },
  addButton: {
      backgroundColor: Colors.primary,
      width: 40,
      height: 40,
      borderRadius: 20,
      alignItems: 'center',
      justifyContent: 'center',
  },
  searchContainer: {
      flexDirection: 'row',
      alignItems: 'center',
      backgroundColor: 'white',
      margin: Spacing.lg,
      paddingHorizontal: Spacing.md,
      borderRadius: BorderRadius.md,
      height: 45,
      borderWidth: 1,
      borderColor: Colors.border,
  },
  searchInput: {
      flex: 1,
      marginLeft: Spacing.sm,
      fontSize: FontSize.md,
  },
  list: {
      paddingHorizontal: Spacing.lg,
      paddingBottom: Spacing.xl,
  },
  userCard: {
      backgroundColor: 'white',
      padding: Spacing.md,
      borderRadius: BorderRadius.md,
      marginBottom: Spacing.md,
      flexDirection: 'row',
      justifyContent: 'space-between',
      alignItems: 'center',
      shadowColor: '#000',
      shadowOffset: { width: 0, height: 2 },
      shadowOpacity: 0.1,
      shadowRadius: 4,
      elevation: 3,
  },
  userInfo: {
      flex: 1,
  },
  userName: {
      fontWeight: 'bold',
      fontSize: FontSize.md,
      color: Colors.text,
  },
  userEmail: {
      color: Colors.textSecondary,
      fontSize: FontSize.sm,
  },
  userPhone: {
      color: Colors.textSecondary,
      fontSize: FontSize.sm,
  },
  actions: {
      flexDirection: 'row',
      gap: Spacing.sm,
  },
  iconBtn: {
      padding: Spacing.xs,
  },
  modalOverlay: {
      flex: 1,
      backgroundColor: 'rgba(0,0,0,0.5)',
      justifyContent: 'center',
      padding: Spacing.lg,
  },
  modalContent: {
      backgroundColor: 'white',
      borderRadius: BorderRadius.lg,
      padding: Spacing.lg,
  },
  modalTitle: {
      fontSize: FontSize.lg,
      fontWeight: 'bold',
      marginBottom: Spacing.lg,
      textAlign: 'center',
  },
  input: {
      borderWidth: 1,
      borderColor: Colors.border,
      borderRadius: BorderRadius.md,
      padding: Spacing.md,
      marginBottom: Spacing.md,
      fontSize: FontSize.md,
  },
  modalActions: {
      flexDirection: 'row',
      justifyContent: 'space-between',
      gap: Spacing.md,
      marginTop: Spacing.md,
  },
  btn: {
      flex: 1,
      padding: Spacing.md,
      borderRadius: BorderRadius.md,
      alignItems: 'center',
  },
  btnCancel: {
      backgroundColor: Colors.border,
  },
  btnSave: {
      backgroundColor: Colors.primary,
  },
  btnText: {
      fontWeight: 'bold',
  },
  pickerContainer: {
      marginBottom: Spacing.md,
  },
  pickerLabel: {
      fontSize: FontSize.sm,
      color: Colors.textSecondary,
      marginBottom: Spacing.xs,
  },
  deptScroll: {
      flexDirection: 'row',
  },
  deptChip: {
      paddingHorizontal: Spacing.md,
      paddingVertical: Spacing.sm,
      borderRadius: BorderRadius.full,
      backgroundColor: Colors.border,
      marginRight: Spacing.sm,
  },
  deptChipActive: {
      backgroundColor: Colors.primary,
  },
  deptChipText: {
      fontSize: FontSize.sm,
      color: Colors.text,
  },
  deptChipTextActive: {
      color: 'white',
  },
});
