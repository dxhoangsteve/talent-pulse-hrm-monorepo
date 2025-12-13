import React, { useState, useEffect } from 'react';
import { View, Text, StyleSheet, ScrollView, TouchableOpacity, Alert } from 'react-native';
import { useAuth } from '../context/AuthContext';
import { Colors, Spacing, FontSize, BorderRadius } from '../constants/theme';
import { 
  Users, 
  DollarSign, 
  Calendar, 
  LogOut,
  Bell,
  ChevronRight,
  Eye,
  Clock,
  MapPin,
  FileText,
  CheckCircle
} from 'lucide-react-native';
import { SafeAreaView } from 'react-native-safe-area-context';

import { NativeStackScreenProps } from '@react-navigation/native-stack';
import { RootStackParamList } from '../types/types';
import { salaryService, SalaryVm } from '../services/salaryService';

type Props = NativeStackScreenProps<RootStackParamList, 'ManagerDashboard'>;

export default function ManagerDashboard({ navigation }: Props) {
  const { user, logout } = useAuth();
  const [paidSalary, setPaidSalary] = useState<SalaryVm | null>(null);
  const [showSalaryNotification, setShowSalaryNotification] = useState(false);

  useEffect(() => {
    checkPaidSalary();
  }, []);

  const checkPaidSalary = async () => {
    const result = await salaryService.getMySalary();
    if (result.isSuccessed && result.resultObj && result.resultObj.length > 0) {
      const paid = result.resultObj.find((s: SalaryVm) => s.status === 'Paid');
      if (paid) {
        setPaidSalary(paid);
        setShowSalaryNotification(true);
      }
    }
  };

  return (
    <SafeAreaView style={styles.container}>
      {/* Header */}
      <View style={styles.header}>
        <View>
          <Text style={styles.welcomeText}>Xin chào,</Text>
          <Text style={styles.userName}>{user?.name || 'Trưởng phòng'}</Text>
          <Text style={styles.roleText}>Quản lý phòng ban</Text>
        </View>
        <View style={styles.headerActions}>
          <TouchableOpacity style={styles.iconButton}>
            <Bell size={24} color={Colors.text} />
          </TouchableOpacity>
          <TouchableOpacity style={styles.iconButton} onPress={logout}>
            <LogOut size={24} color={Colors.error} />
          </TouchableOpacity>
        </View>
      </View>

      <ScrollView style={styles.content} contentContainerStyle={styles.scrollContent}>
        {/* Salary Notification */}
        {showSalaryNotification && paidSalary && (
          <TouchableOpacity 
            style={styles.salaryNotification}
            onPress={() => setShowSalaryNotification(false)}
          >
            <CheckCircle size={24} color={Colors.success} />
            <View style={styles.salaryNotificationContent}>
              <Text style={styles.salaryNotificationTitle}>
                🎉 Lương tháng {paidSalary.month}/{paidSalary.year} đã được thanh toán!
              </Text>
              <Text style={styles.salaryNotificationAmount}>
                Thực nhận: {paidSalary.netSalary?.toLocaleString('vi-VN')}đ
              </Text>
            </View>
          </TouchableOpacity>
        )}

        {/* Personal Actions - Like Employee */}
        <Text style={styles.sectionTitle}>Dành cho bạn</Text>
        <View style={styles.quickGrid}>
          <TouchableOpacity 
            style={styles.quickItem}
            onPress={() => navigation.navigate('AttendanceScreen')}
          >
            <View style={[styles.quickIcon, { backgroundColor: '#10B98115' }]}>
              <MapPin size={24} color="#10B981" />
            </View>
            <Text style={styles.quickLabel}>Chấm công</Text>
          </TouchableOpacity>
          
          <TouchableOpacity 
            style={styles.quickItem}
            onPress={() => navigation.navigate('LeaveRequest')}
          >
            <View style={[styles.quickIcon, { backgroundColor: Colors.primaryLight }]}>
              <Calendar size={24} color={Colors.primary} />
            </View>
            <Text style={styles.quickLabel}>Xin nghỉ</Text>
          </TouchableOpacity>
          
          <TouchableOpacity 
            style={styles.quickItem}
            onPress={() => navigation.navigate('OTRequest')}
          >
            <View style={[styles.quickIcon, { backgroundColor: Colors.secondary + '15' }]}>
              <Clock size={24} color={Colors.secondary} />
            </View>
            <Text style={styles.quickLabel}>Đăng ký OT</Text>
          </TouchableOpacity>
          
          <TouchableOpacity 
            style={styles.quickItem}
            onPress={() => navigation.navigate('MySalary')}
          >
            <View style={[styles.quickIcon, { backgroundColor: '#F59E0B15' }]}>
              <DollarSign size={24} color="#F59E0B" />
            </View>
            <Text style={styles.quickLabel}>Lương của tôi</Text>
          </TouchableOpacity>
        </View>

        {/* Management Actions */}
        <Text style={styles.sectionTitle}>Quản lý phòng ban</Text>
        <View style={styles.actionList}>
          <ActionItem 
            title="Duyệt đơn nghỉ phép & OT" 
            subtitle="Phê duyệt yêu cầu của nhân viên"
            icon={<FileText size={24} color="white" />}
            color={Colors.secondary}
            onPress={() => navigation.navigate('ApprovalScreen')}
          />
          <ActionItem 
            title="Xem nhân viên phòng ban" 
            subtitle="Danh sách nhân viên"
            icon={<Users size={24} color="white" />}
            color={Colors.primary}
            onPress={() => navigation.navigate('DepartmentEmployees')}
          />
          <ActionItem 
            title="Lịch sử chấm công" 
            subtitle="Xem chấm công nhân viên phòng ban"
            icon={<Clock size={24} color="white" />}
            color="#10B981"
            onPress={() => navigation.navigate('AttendanceHistory')}
          />
        </View>

        {/* Info Notice */}
        <View style={styles.infoBox}>
          <Eye size={20} color={Colors.secondary} />
          <Text style={styles.infoText}>
            Bạn có thể xem và duyệt đơn của nhân viên trong phòng ban mình quản lý.
          </Text>
        </View>
      </ScrollView>
    </SafeAreaView>
  );
}

const ActionItem = ({ title, subtitle, icon, color, onPress }: any) => (
  <TouchableOpacity style={styles.actionItem} onPress={onPress}>
    <View style={[styles.actionIcon, { backgroundColor: color }]}>
      {icon}
    </View>
    <View style={styles.actionContent}>
      <Text style={styles.actionTitle}>{title}</Text>
      <Text style={styles.actionSubtitle}>{subtitle}</Text>
    </View>
    <ChevronRight size={20} color={Colors.textSecondary} />
  </TouchableOpacity>
);

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  header: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    padding: Spacing.lg,
    backgroundColor: Colors.card,
  },
  welcomeText: {
    fontSize: FontSize.sm,
    color: Colors.textSecondary,
  },
  userName: {
    fontSize: FontSize.xl,
    fontWeight: 'bold',
    color: Colors.text,
    marginTop: 2,
  },
  roleText: {
    fontSize: FontSize.xs,
    color: Colors.secondary,
    marginTop: 4,
  },
  headerActions: {
    flexDirection: 'row',
    gap: 8,
  },
  iconButton: {
    padding: Spacing.sm,
  },
  content: {
    flex: 1,
  },
  scrollContent: {
    padding: Spacing.lg,
  },
  sectionTitle: {
    fontSize: FontSize.lg,
    fontWeight: '600',
    color: Colors.text,
    marginBottom: Spacing.md,
    marginTop: Spacing.sm,
  },
  // Quick Grid for personal actions
  quickGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: Spacing.md,
    marginBottom: Spacing.lg,
  },
  quickItem: {
    width: '47%',
    backgroundColor: Colors.card,
    borderRadius: BorderRadius.lg,
    padding: Spacing.md,
    alignItems: 'center',
  },
  quickIcon: {
    width: 48,
    height: 48,
    borderRadius: 24,
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: Spacing.sm,
  },
  quickLabel: {
    fontSize: FontSize.sm,
    fontWeight: '500',
    color: Colors.text,
  },
  // Action list for management
  actionList: {
    gap: Spacing.sm,
  },
  actionItem: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.card,
    padding: Spacing.md,
    borderRadius: BorderRadius.lg,
  },
  actionIcon: {
    width: 48,
    height: 48,
    borderRadius: BorderRadius.md,
    alignItems: 'center',
    justifyContent: 'center',
    marginRight: Spacing.md,
  },
  actionContent: {
    flex: 1,
  },
  actionTitle: {
    fontSize: FontSize.md,
    fontWeight: '600',
    color: Colors.text,
  },
  actionSubtitle: {
    fontSize: FontSize.sm,
    color: Colors.textSecondary,
    marginTop: 2,
  },
  infoBox: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.secondary + '15',
    padding: Spacing.md,
    borderRadius: BorderRadius.md,
    marginTop: Spacing.xl,
    gap: Spacing.sm,
  },
  infoText: {
    flex: 1,
    fontSize: FontSize.sm,
    color: Colors.secondary,
  },
  // Salary notification
  salaryNotification: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: '#10B98115',
    padding: Spacing.md,
    borderRadius: BorderRadius.md,
    marginBottom: Spacing.md,
    gap: Spacing.sm,
  },
  salaryNotificationContent: {
    flex: 1,
  },
  salaryNotificationTitle: {
    fontSize: FontSize.sm,
    fontWeight: '600',
    color: Colors.text,
  },
  salaryNotificationAmount: {
    fontSize: FontSize.xs,
    color: Colors.success,
    marginTop: 2,
  },
});
