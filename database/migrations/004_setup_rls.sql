-- Enable RLS on user-related tables
ALTER TABLE users ENABLE ROW LEVEL SECURITY;
ALTER TABLE roles ENABLE ROW LEVEL SECURITY;
ALTER TABLE user_roles ENABLE ROW LEVEL SECURITY;
ALTER TABLE refresh_tokens ENABLE ROW LEVEL SECURITY;
ALTER TABLE audit_logs ENABLE ROW LEVEL SECURITY;

-- Create RLS policies for users table
CREATE POLICY tenant_isolation_users ON users
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

-- Create RLS policies for roles table
CREATE POLICY tenant_isolation_roles ON roles
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

-- Create RLS policies for user_roles table
CREATE POLICY tenant_isolation_user_roles ON user_roles
    USING (EXISTS (
        SELECT 1 FROM users u WHERE u.id = user_roles.user_id 
        AND u.tenant_id = current_setting('app.current_tenant_id')::UUID
    ));

-- Create RLS policies for refresh_tokens table
CREATE POLICY tenant_isolation_refresh_tokens ON refresh_tokens
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);

-- Create RLS policies for audit_logs table
CREATE POLICY tenant_isolation_audit_logs ON audit_logs
    USING (tenant_id = current_setting('app.current_tenant_id')::UUID);