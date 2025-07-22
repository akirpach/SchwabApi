-- Function to update updated_at timestamp
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$ language 'plpgsql';


-- Triggers for updated_at
CREATE TRIGGER update_tenants_updated_at BEFORE UPDATE ON tenants
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_users_updated_at BEFORE UPDATE ON users
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();


-- Function to create default roles for new tenant
CREATE OR REPLACE FUNCTION create_default_tenant_roles()
RETURNS TRIGGER AS $$
BEGIN
    -- Create default Admin role
    INSERT INTO roles (tenant_id, name, display_name, description, is_system_role, permissions)
    VALUES (
        NEW.id,
        'admin',
        'Administrator',
        'Full access to all features and settings',
        TRUE,
        '["*"]'::jsonb
    );

     -- Create default User role
    INSERT INTO roles (tenant_id, name, display_name, description, is_system_role, permissions)
    VALUES (
        NEW.id,
        'user',
        'User',
        'Standard user access',
        TRUE,
        '["read:profile", "update:profile"]'::jsonb
    );
    
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Trigger to create default roles
CREATE TRIGGER create_tenant_default_roles
    AFTER INSERT ON tenants
    FOR EACH ROW EXECUTE FUNCTION create_default_tenant_roles();


-- Function to automatically assign admin role to first user
CREATE OR REPLACE FUNCTION assign_first_user_admin_role()
RETURNS TRIGGER AS $$
DECLARE
    admin_role_id UUID;
    user_count INTEGER;
BEGIN
    -- Check if this is the first user for the tenant
    SELECT COUNT(*) INTO user_count FROM users WHERE tenant_id = NEW.tenant_id;
    
    IF user_count = 1 THEN
        -- Get the admin role for this tenant
        SELECT id INTO admin_role_id 
        FROM roles 
        WHERE tenant_id = NEW.tenant_id AND name = 'admin';
        
        -- Assign admin role to the first user
        INSERT INTO user_roles (user_id, role_id)
        VALUES (NEW.id, admin_role_id);
    END IF;
    
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Trigger to assign admin role to first user
CREATE TRIGGER assign_first_user_admin
    AFTER INSERT ON users
    FOR EACH ROW EXECUTE FUNCTION assign_first_user_admin_role();

-- Function to clean up expired tokens
CREATE OR REPLACE FUNCTION cleanup_expired_tokens()
RETURNS INTEGER AS $$
DECLARE
    deleted_count INTEGER;
BEGIN
    DELETE FROM refresh_tokens WHERE expires_at < NOW();
    GET DIAGNOSTICS deleted_count = ROW_COUNT;
    RETURN deleted_count;
END;
$$ language 'plpgsql';