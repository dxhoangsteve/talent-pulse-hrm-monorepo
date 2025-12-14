import React, { useState, useEffect } from 'react';
import { View, Text, StyleSheet, ScrollView, TouchableOpacity, Image } from 'react-native';
import { useAuth } from '../context/AuthContext';
import { Colors, Spacing, FontSize, BorderRadius } from '../constants/theme';
import {
  LogOut,
  Bell,
  Clock,
  Calendar,
  FileText,
  Briefcase,
  DollarSign,
  X
} from 'lucide-react-native';
import { SafeAreaView } from 'react-native-safe-area-context';
import { useNavigation } from '@react-navigation/native';
import { NativeStackNavigationProp } from '@react-navigation/native-stack';
import { RootStackParamList } from '../types/types';
import { salaryService, SalaryVm } from '../services/salaryService';

type NavigationProp = NativeStackNavigationProp<RootStackParamList>;

export default function EmployeeDashboard() {
  const { user, logout } = useAuth();
  const navigation = useNavigation<NavigationProp>();
  const [paidSalary, setPaidSalary] = useState<SalaryVm | null>(null);
  const [showSalaryNotify, setShowSalaryNotify] = useState(false);

  useEffect(() => {
    checkPaidSalary();
  }, []);

  const checkPaidSalary = async () => {
    const salary = await salaryService.getLatestPaidSalary();
    if (salary) {
      setPaidSalary(salary);
      setShowSalaryNotify(true);
    }
  };

  const today = new Date().toLocaleDateString('vi-VN', { weekday: 'long', day: 'numeric', month: 'long' });

  /* Real-time Clock */
  const [currentTime, setCurrentTime] = useState(new Date());

  useEffect(() => {
    const timer = setInterval(() => {
      setCurrentTime(new Date());
    }, 1000);
    return () => clearInterval(timer);
  }, []);

  const formatTime = (date: Date) => {
    return date.toLocaleTimeString('vi-VN', { hour12: false });
  };

  return (
    <SafeAreaView style={styles.container}>
      {/* Header */}
      <View style={styles.header}>
        <View style={styles.userInfo}>
          <View style={styles.avatar}>
            <Text style={styles.avatarText}>{user?.name?.charAt(0) || 'E'}</Text>
          </View>
          <View>
            <Text style={styles.greeting}>Xin chào,</Text>
            <Text style={styles.userName}>{user?.name || 'Nhân viên'}</Text>
          </View>
        </View>
        <TouchableOpacity onPress={logout} style={styles.logoutBtn}>
          <LogOut size={20} color={Colors.textSecondary} />
        </TouchableOpacity>
      </View>

      <ScrollView contentContainerStyle={styles.content}>
        {/* Salary Paid Notification */}
        {showSalaryNotify && paidSalary && (
          <View style={styles.salaryNotify}>
            <DollarSign size={24} color={Colors.success} />
            <View style={styles.salaryNotifyContent}>
              <Text style={styles.salaryNotifyTitle}>Lương đã được phát!</Text>
              <Text style={styles.salaryNotifyText}>
                Tháng {paidSalary.month}/{paidSalary.year}: {paidSalary.netSalary.toLocaleString('vi-VN')}đ
              </Text>
            </View>
            <TouchableOpacity onPress={() => setShowSalaryNotify(false)}>
              <X size={20} color={Colors.textSecondary} />
            </TouchableOpacity>
          </View>
        )}

        {/* Check-in Card */}
        <View style={styles.checkInCard}>
          <Text style={styles.dateText}>{today}</Text>
          {/* <Text style={styles.shiftText}>Ca làm việc: 08:00 - 17:00</Text> */}

          <View style={styles.timerContainer}>
            <Text style={styles.timerText}>{formatTime(currentTime)}</Text>
            <Text style={styles.timerLabel}>Giờ hiện tại</Text>
          </View>

          <TouchableOpacity
            style={styles.checkInButton}
            onPress={() => navigation.navigate('AttendanceScreen')}
          >
            <View style={styles.fingerprintIcon}>
              <Clock size={32} color="white" />
            </View>
            <Text style={styles.checkInButtonText}>Chấm công GPS</Text>
          </TouchableOpacity>
        </View>

        {/* Menu Grid */}
        <Text style={styles.sectionTitle}>Tiện ích</Text>
        <View style={styles.menuGrid}>
          <MenuItem
            title="Xin nghỉ phép"
            icon={<FileText size={24} color="#10B981" />}
            onPress={() => navigation.navigate('LeaveRequest')}
          />
          <MenuItem
            title="Đăng ký OT"
            icon={<Clock size={24} color="#6366F1" />}
            onPress={() => navigation.navigate('OTRequest')}
          />
          <MenuItem
            title="Phiếu lương"
            icon={<Briefcase size={24} color="#F59E0B" />}
            onPress={() => navigation.navigate('MySalary')}
          />
        </View>

        {/* Recent Activity Removed as requested */}

      </ScrollView>

    </SafeAreaView >
  );
}

const MenuItem = ({ title, icon, onPress }: { title: string; icon: React.ReactNode; onPress?: () => void }) => (
  <TouchableOpacity style={styles.menuItem} onPress={onPress}>
    <View style={styles.menuIconContainer}>
      {icon}
    </View>
    <Text style={styles.menuTitle}>{title}</Text>
  </TouchableOpacity>
);

const ActivityItem = ({ title, time, status }: any) => (
  <View style={styles.activityItem}>
    <View style={styles.activityInfo}>
      <Text style={styles.activityTitle}>{title}</Text>
      <Text style={styles.activityTime}>{time}</Text>
    </View>
    <View style={[styles.statusDot, { backgroundColor: status === 'success' ? Colors.success : Colors.warning }]} />
  </View>
)

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  header: {
    padding: Spacing.lg,
    backgroundColor: 'white',
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  userInfo: {
    flexDirection: 'row',
    alignItems: 'center',
  },
  avatar: {
    width: 48,
    height: 48,
    borderRadius: 24,
    backgroundColor: Colors.primaryLight,
    alignItems: 'center',
    justifyContent: 'center',
    marginRight: Spacing.md,
  },
  avatarText: {
    fontSize: FontSize.lg,
    fontWeight: 'bold',
    color: 'white',
  },
  greeting: {
    fontSize: FontSize.xs,
    color: Colors.textSecondary,
  },
  userName: {
    fontSize: FontSize.md,
    fontWeight: 'bold',
    color: Colors.text,
  },
  logoutBtn: {
    padding: Spacing.sm,
  },
  content: {
    padding: Spacing.lg,
  },
  checkInCard: {
    backgroundColor: Colors.primary,
    borderRadius: BorderRadius.lg,
    padding: Spacing.xl,
    alignItems: 'center',
    marginBottom: Spacing.xl,
  },
  dateText: {
    color: 'rgba(255,255,255,0.9)',
    fontSize: FontSize.md,
    marginBottom: Spacing.xs,
  },
  shiftText: {
    color: 'rgba(255,255,255,0.7)',
    fontSize: FontSize.sm,
    marginBottom: Spacing.lg,
  },
  timerContainer: {
    alignItems: 'center',
    marginBottom: Spacing.xl,
  },
  timerText: {
    fontSize: 48,
    fontWeight: 'bold',
    color: 'white',
    fontVariant: ['tabular-nums'],
  },
  timerLabel: {
    color: 'rgba(255,255,255,0.7)',
    fontSize: FontSize.xs,
  },
  checkInButton: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: 'rgba(255,255,255,0.2)',
    paddingVertical: Spacing.md,
    paddingHorizontal: Spacing.xl,
    borderRadius: BorderRadius.full,
    borderWidth: 1,
    borderColor: 'rgba(255,255,255,0.3)',
  },
  fingerprintIcon: {
    marginRight: Spacing.sm,
  },
  checkInButtonText: {
    color: 'white',
    fontWeight: 'bold',
    fontSize: FontSize.md,
  },
  sectionTitle: {
    fontSize: FontSize.lg,
    fontWeight: 'bold',
    color: Colors.text,
    marginBottom: Spacing.md,
  },
  menuGrid: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: Spacing.md,
    marginBottom: Spacing.xl,
  },
  menuItem: {
    width: '47%',
    backgroundColor: 'white',
    padding: Spacing.md,
    borderRadius: BorderRadius.md,
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.05,
    shadowRadius: 4,
    elevation: 2,
  },
  menuIconContainer: {
    width: 48,
    height: 48,
    borderRadius: 24,
    backgroundColor: Colors.background,
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: Spacing.sm,
  },
  menuTitle: {
    fontSize: FontSize.sm,
    fontWeight: '600',
    color: Colors.text,
  },
  activityList: {
    backgroundColor: 'white',
    borderRadius: BorderRadius.md,
    padding: Spacing.md,
  },
  activityItem: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingVertical: Spacing.sm,
    borderBottomWidth: 1,
    borderBottomColor: Colors.background,
  },
  activityInfo: {

  },
  activityTitle: {
    fontWeight: '600',
    color: Colors.text,
    fontSize: FontSize.sm,
  },
  activityTime: {
    color: Colors.textSecondary,
    fontSize: FontSize.xs,
  },
  statusDot: {
    width: 8,
    height: 8,
    borderRadius: 4,
  },
  salaryNotify: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.success + '15',
    padding: Spacing.md,
    borderRadius: BorderRadius.md,
    marginBottom: Spacing.md,
    borderWidth: 1,
    borderColor: Colors.success + '30',
  },
  salaryNotifyContent: {
    flex: 1,
    marginLeft: Spacing.sm,
  },
  salaryNotifyTitle: {
    fontWeight: '600',
    color: Colors.success,
    fontSize: FontSize.sm,
  },
  salaryNotifyText: {
    color: Colors.text,
    fontSize: FontSize.xs,
    marginTop: 2,
  },
});
