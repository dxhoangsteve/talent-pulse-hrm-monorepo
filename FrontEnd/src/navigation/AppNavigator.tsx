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
import UserManagementScreen from '../screens/UserManagementScreen';
import LeaveRequestScreen from '../screens/LeaveRequestScreen';
import OTRequestScreen from '../screens/OTRequestScreen';
import ApprovalScreen from '../screens/ApprovalScreen';
import DepartmentManagementScreen from '../screens/DepartmentManagementScreen';

const Stack = createNativeStackNavigator<RootStackParamList>();

export default function AppNavigator() {
  const { token, isLoading, isAdmin } = useAuth();

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
          </>
        ) : (
          <>
            <Stack.Screen name="EmployeeDashboard" component={EmployeeDashboard} />
            <Stack.Screen name="LeaveRequest" component={LeaveRequestScreen} />
            <Stack.Screen name="OTRequest" component={OTRequestScreen} />
          </>
        )}
      </Stack.Navigator>
    </NavigationContainer>
  );
}



