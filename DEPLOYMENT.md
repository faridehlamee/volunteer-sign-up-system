# Deployment Guide - Volunteer Sign-Up System

This guide will help you deploy the Volunteer Sign-Up System to various platforms including Railway, Vercel, and traditional servers.

## Platform Compatibility

- ✅ **Railway** - Fully supported (recommended for easy deployment)
- ❌ **Vercel** - Not supported (Vercel doesn't support .NET applications)
- ✅ **Traditional Servers** - Windows IIS or Linux (Nginx/systemd)
- ✅ **Other Cloud Platforms** - Azure App Service, AWS Elastic Beanstalk, Heroku, etc.

## Prerequisites

- .NET 8.0 Runtime installed on your server
- Access to your web server (Windows IIS or Linux)
- Domain/subdomain configured (e.g., `volunteers.yourdomain.com`)
- Resend API key and verified domain

## Quick Deploy: Railway (Recommended)

Railway is the easiest way to deploy this .NET application. It automatically detects .NET projects and handles deployment.

### Step 1: Prepare Your Repository

1. **Push your code to GitHub/GitLab/Bitbucket**
   ```bash
   git remote add origin <your-repository-url>
   git push -u origin main
   ```

### Step 2: Deploy to Railway

1. **Sign up/Login to Railway**
   - Go to [railway.app](https://railway.app)
   - Sign up with GitHub (recommended)

2. **Create a New Project**
   - Click "New Project"
   - Select "Deploy from GitHub repo"
   - Choose your repository

3. **Configure Environment Variables**
   - Go to your project → Variables tab
   - Add the following environment variables:
     ```
     ASPNETCORE_ENVIRONMENT=Production
     Resend__ApiKey=re_your_api_key_here
     Resend__FromEmail=noreply@kiatechsoftware.com
     Resend__AdminEmail=info@kiatechsoftware.com
     Resend__UseTestRecipient=false
     ```

4. **Configure Custom Domain (Optional)**
   - Go to Settings → Networking
   - Add your custom domain (e.g., `volunteers.kiatechsoftware.com`)
   - Railway will provide DNS records to add

5. **Deploy**
   - Railway will automatically detect the .NET project
   - It will build and deploy your application
   - Your app will be live at `https://your-app-name.up.railway.app`

### Railway Features

- ✅ Automatic HTTPS
- ✅ Free tier available ($5 credit/month)
- ✅ Automatic deployments from Git
- ✅ Environment variable management
- ✅ Custom domains
- ✅ Persistent storage for SQLite database

### Railway Pricing

- **Hobby Plan**: $5/month credit (good for small projects)
- **Pro Plan**: $20/month + usage
- Free tier available for testing

---

## Step 1: Publish the Application (Traditional Deployment)

### Option A: Publish for Windows IIS

```powershell
# Navigate to your project directory
cd "c:\volunteer sign-up system"

# Publish the application
dotnet publish -c Release -o ./publish
```

This creates a `publish` folder with all necessary files.

### Option B: Publish for Linux

```bash
# Navigate to your project directory
cd /path/to/volunteer-sign-up-system

# Publish for Linux (self-contained or framework-dependent)
dotnet publish -c Release -o ./publish -r linux-x64
# OR for framework-dependent (requires .NET runtime on server):
dotnet publish -c Release -o ./publish
```

## Step 2: Configure Production Settings

1. **Update `appsettings.Production.json`** with your production settings:
   - Set `FromEmail` to your verified domain email (e.g., `noreply@kiatechsoftware.com`)
   - Set `UseTestRecipient` to `false`
   - Verify `AdminEmail` is correct

2. **Database Location**: The SQLite database (`volunteers.db`) will be created in the application root directory. Make sure the application has write permissions.

## Step 3: Deploy to Windows IIS

### 3.1 Copy Files to Server

Copy the contents of the `publish` folder to your IIS website directory (e.g., `C:\inetpub\wwwroot\volunteers`).

### 3.2 Configure IIS

1. Open **IIS Manager**
2. Create a new **Application Pool**:
   - Name: `VolunteerSignUpAppPool`
   - .NET CLR Version: **No Managed Code** (for .NET 8)
   - Managed Pipeline Mode: **Integrated**

3. Create a new **Website** or **Application**:
   - Physical Path: Point to your published files
   - Application Pool: Select `VolunteerSignUpAppPool`
   - Binding: 
     - Type: `http` or `https`
     - IP Address: `All Unassigned`
     - Port: `80` (or `443` for HTTPS)
     - Host name: `volunteers.yourdomain.com` (your subdomain)

4. **Set Permissions**:
   - Give `IIS_IUSRS` read/execute permissions
   - Give `IIS_IUSRS` write permissions to the folder (for SQLite database)

### 3.3 Install .NET Hosting Bundle

Download and install the **.NET 8.0 Hosting Bundle** from:
https://dotnet.microsoft.com/download/dotnet/8.0

This includes the .NET runtime and ASP.NET Core module for IIS.

### 3.4 Configure Environment Variable

Set the environment variable `ASPNETCORE_ENVIRONMENT=Production` in IIS:
- Right-click your website → **Configuration Editor**
- Navigate to `system.webServer/aspNetCore`
- Set `environmentVariables` → `ASPNETCORE_ENVIRONMENT` = `Production`

## Step 4: Deploy to Linux (Nginx + systemd)

### 4.1 Copy Files to Server

```bash
# Copy published files to server
scp -r ./publish/* user@yourserver:/var/www/volunteers/
```

### 4.2 Create systemd Service

Create `/etc/systemd/system/volunteers.service`:

```ini
[Unit]
Description=Volunteer Sign-Up System
After=network.target

[Service]
Type=notify
WorkingDirectory=/var/www/volunteers
ExecStart=/usr/bin/dotnet /var/www/volunteers/VolunteerSignUpSystem.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=volunteers
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

Enable and start the service:

```bash
sudo systemctl enable volunteers
sudo systemctl start volunteers
sudo systemctl status volunteers
```

### 4.3 Configure Nginx

Create `/etc/nginx/sites-available/volunteers`:

```nginx
server {
    listen 80;
    server_name volunteers.yourdomain.com;

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

Enable the site:

```bash
sudo ln -s /etc/nginx/sites-available/volunteers /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```

### 4.4 Set Up SSL (Let's Encrypt)

```bash
sudo apt install certbot python3-certbot-nginx
sudo certbot --nginx -d volunteers.yourdomain.com
```

## Step 5: Configure DNS

Add an **A record** or **CNAME** for your subdomain:

- **Type**: A (if pointing to IP) or CNAME (if pointing to another domain)
- **Name**: `volunteers` (or your subdomain name)
- **Value**: Your server's IP address or hostname
- **TTL**: 3600 (or default)

## Step 6: Verify Deployment

1. Visit `http://volunteers.yourdomain.com` (or `https://` if SSL is configured)
2. Test the volunteer sign-up form
3. Check that emails are being sent
4. Verify the database is being created and data is stored

## Step 7: Security Considerations

### Important Security Steps:

1. **Remove Development Files**: Don't deploy `appsettings.Development.json` to production
2. **Secure API Keys**: Consider using environment variables or Azure Key Vault for sensitive data
3. **HTTPS**: Always use HTTPS in production
4. **Database Backup**: Set up regular backups of `volunteers.db`
5. **File Permissions**: Restrict file permissions on the server

### Using Environment Variables (Recommended)

Instead of storing API keys in `appsettings.Production.json`, use environment variables:

**Windows IIS:**
- Set in IIS Configuration Editor or web.config

**Linux:**
- Add to systemd service file:
  ```ini
  Environment=Resend__ApiKey=re_your_api_key_here
  Environment=Resend__FromEmail=noreply@kiatechsoftware.com
  ```

## Troubleshooting

### Application Won't Start

- Check logs: `dotnet run` output or systemd logs (`journalctl -u volunteers`)
- Verify .NET runtime is installed: `dotnet --version`
- Check file permissions
- Verify database folder has write permissions

### Database Issues

- Ensure the application folder has write permissions
- Check that `volunteers.db` is being created
- Verify SQLite is working: `sqlite3 volunteers.db "SELECT * FROM VolunteerSignUps;"`

### Email Not Sending

- Verify Resend API key is correct
- Check domain verification status in Resend dashboard
- Ensure `FromEmail` uses your verified domain
- Check application logs for email errors

### 404 Errors

- Verify IIS/Nginx configuration
- Check URL rewrite rules
- Ensure application is running on the correct port

## Maintenance

### Updating the Application

1. Publish new version: `dotnet publish -c Release -o ./publish`
2. Stop the service/application
3. Backup `volunteers.db`
4. Copy new files to server
5. Restart the service/application

### Database Backup

```bash
# Linux
cp /var/www/volunteers/volunteers.db /backup/volunteers-$(date +%Y%m%d).db

# Windows
copy C:\inetpub\wwwroot\volunteers\volunteers.db C:\backup\volunteers-%date%.db
```

## Support

For issues or questions, check:
- Application logs
- Server logs
- Resend dashboard for email delivery status

