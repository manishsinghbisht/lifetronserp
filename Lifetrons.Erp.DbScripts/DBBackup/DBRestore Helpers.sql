Use LtSysDb1
EXEC sp_change_users_login 'UPDATE_ONE','msb','msb';
GRANT EXECUTE ON [dbo].[spConvertLeadToTasknOpp] TO [msb];
GRANT EXECUTE ON [dbo].[sViewDashboardOpenWork] TO [msb];
GRANT EXECUTE ON [dbo].[sViewDashboardMonthlyLeadOppComaprison] TO [msb];