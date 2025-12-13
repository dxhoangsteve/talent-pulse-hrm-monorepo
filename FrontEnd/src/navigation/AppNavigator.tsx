import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { ActivityIndicator, View } from 'react-native';

import { useAuth } from '../context/AuthContext';
import { RootStackParamList } from '../types/types';
import { Colors } from '../constants/theme';

import LoginScreen from '../screens/LoginScreen';
import AdminDashboard from '../screens/AdminDashboard';
import EmployeeDashboard from '../screens/EmployeeDashboard';
import ManagerDashboard from '../screens/ManagerDashboard';
import UserManagementScreen from '../screens/UserManagementScreen';
import LeaveRequestScreen from '../screens/LeaveRequestScreen';
import OTRequestScreen from '../screens/OTRequestScreen';
import ApprovalScreen from '../screens/ApprovalScreen';
import DepartmentManagementScreen from '../screens/DepartmentManagementScreen';
import AttendanceScreen from '../screens/AttendanceScreen';
import AttendanceHistoryScreen from '../screens/AttendanceHistoryScreen';
import LeaveHistoryScreen from '../screens/LeaveHistoryScreen';
import OTHistoryScreen from '../screens/OTHistoryScreen';
import SalaryHistoryScreen from '../screens/SalaryHistoryScreen';
import MySalaryScreen from '../screens/MySalaryScreen';

const Stack = createNativeStackNavigator<RootStackParamList>();

export default function AppNavigator() {
  const { token, isLoading, isAdmin, isManager } = useAuth();

  if (isLoading) {
    return (
      <View style={{ flex: 1, justifyContent: 'center', alignItems: 'center' }}>
        <ActivityIndicator size="large" color={Colors.primary} />
      </View>
    );
  }

  return (
    <NavigationContainer>
      <Stack.Navigator screenOptions={{ headerShown: false }}>
        {!token ? (
          <Stack.Screen name="Login" component={LoginScreen} />
        ) : isAdmin ? (
          <>
            <Stack.Screen name="AdminDashboard" component={AdminDashboard} />
            <Stack.Screen name="UserManagement" component={UserManagementScreen} />
            <Stack.Screen name="LeaveRequest" component={LeaveRequestScreen} />
            <Stack.Screen name="OTRequest" component={OTRequestScreen} />
            <Stack.Screen name="ApprovalScreen" component={ApprovalScreen} />
            <Stack.Screen name="DepartmentManagement" component={DepartmentManagementScreen} />
            <Stack.Screen name="AttendanceScreen" component={AttendanceScreen} />
            <Stack.Screen name="AttendanceHistory" component={AttendanceHistoryScreen} />
            <Stack.Screen name="LeaveHistory" component={LeaveHistoryScreen} />
            <Stack.Screen name="OTHistory" component={OTHistoryScreen} />
            <Stack.Screen name="SalaryHistory" component={SalaryHistoryScreen} />
          </>
        ) : isManager ? (
          <>
            <Stack.Screen name="ManagerDashboard" component={ManagerDashboard} />
            <Stack.Screen name="ApprovalScreen" component={ApprovalScreen} />
            <Stack.Screen name="AttendanceScreen" component={AttendanceScreen} />
            <Stack.Screen name="AttendanceHistory" component={AttendanceHistoryScreen} />
            <Stack.Screen name="LeaveRequest" component={LeaveRequestScreen} />
            <Stack.Screen name="OTRequest" component={OTRequestScreen} />
          </>
        ) : (
          <>
            <Stack.Screen name="EmployeeDashboard" component={EmployeeDashboard} />
            <Stack.Screen name="LeaveRequest" component={LeaveRequestScreen} />
            <Stack.Screen name="OTRequest" component={OTRequestScreen} />
            <Stack.Screen name="AttendanceScreen" component={AttendanceScreen} />
            <Stack.Screen name="MySalary" component={MySalaryScreen} />
          </>
        )}
      </Stack.Navigator>
    </NavigationContainer>
  );
}






