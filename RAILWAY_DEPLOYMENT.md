# Railway Deployment Guide

This is a quick guide for deploying the Volunteer Sign-Up System to Railway.

## Why Railway?

- ✅ **Easy Setup**: Just connect your Git repository
- ✅ **Automatic Builds**: Detects .NET projects automatically
- ✅ **Free Tier**: $5 credit per month
- ✅ **HTTPS Included**: Automatic SSL certificates
- ✅ **Custom Domains**: Easy subdomain setup
- ✅ **Environment Variables**: Secure configuration management

## Prerequisites

- GitHub/GitLab/Bitbucket account with your code pushed
- Railway account (free at [railway.app](https://railway.app))
- Resend API key

## Step-by-Step Deployment

### 1. Push Your Code to Git

If you haven't already:

```bash
git add .
git commit -m "Ready for Railway deployment"
git push origin main
```

### 2. Sign Up for Railway

1. Go to [railway.app](https://railway.app)
2. Click "Start a New Project"
3. Sign up with GitHub (recommended for easy repo access)

### 3. Create a New Project

1. Click **"New Project"**
2. Select **"Deploy from GitHub repo"**
3. Authorize Railway to access your repositories
4. Select your `volunteer-sign-up-system` repository

### 4. Configure Environment Variables

Railway will automatically start building, but you need to configure your environment variables:

1. Go to your project dashboard
2. Click on the **"Variables"** tab
3. Add the following variables:

| Variable Name | Value | Description |
|--------------|-------|-------------|
| `ASPNETCORE_ENVIRONMENT` | `Production` | Sets the environment |
| `Resend__ApiKey` | `re_your_actual_api_key` | Your Resend API key |
| `Resend__FromEmail` | `noreply@kiatechsoftware.com` | Your verified domain email |
| `Resend__AdminEmail` | `info@kiatechsoftware.com` | Admin notification email |
| `Resend__UseTestRecipient` | `false` | Use actual recipient emails |

**Important**: Use double underscores (`__`) for nested configuration (e.g., `Resend__ApiKey` not `Resend:ApiKey`)

### 5. Wait for Deployment

Railway will:
- Detect your .NET project
- Install .NET 8.0 SDK
- Run `dotnet restore`
- Run `dotnet publish`
- Start your application

You can watch the build logs in real-time.

### 6. Access Your Application

Once deployed, Railway will provide a URL like:
```
https://volunteer-sign-up-system-production.up.railway.app
```

### 7. Set Up Custom Domain (Optional)

1. Go to **Settings** → **Networking**
2. Click **"Custom Domain"**
3. Enter your subdomain: `volunteers.kiatechsoftware.com`
4. Railway will provide DNS records:
   - **Type**: CNAME
   - **Name**: `volunteers`
   - **Value**: `your-app.up.railway.app`
5. Add the CNAME record to your DNS provider
6. Wait for DNS propagation (usually 5-15 minutes)
7. Railway will automatically provision SSL certificate

## Database Persistence

**Important**: Railway's free tier uses **ephemeral storage**. Your SQLite database will be lost when the service restarts.

### Solution Options:

1. **Upgrade to Railway Pro** ($20/month) - Includes persistent volumes
2. **Use Railway PostgreSQL** (Free tier available):
   - Add PostgreSQL service in Railway
   - Update connection string in `Program.cs`
3. **Use External Database**: Azure SQL, AWS RDS, etc.

### Quick Fix for SQLite on Railway:

The database file is stored in the application directory. Railway's free tier will persist it between deployments but not between service restarts. For production, consider:

- Using Railway's PostgreSQL (free tier)
- Or upgrading to Railway Pro for persistent volumes

## Monitoring and Logs

1. **View Logs**: Click on your service → **"Logs"** tab
2. **Metrics**: View CPU, Memory, Network usage
3. **Deployments**: See deployment history and rollback if needed

## Updating Your Application

Railway automatically deploys when you push to your Git repository:

```bash
git add .
git commit -m "Update application"
git push origin main
```

Railway will:
1. Detect the new commit
2. Build the new version
3. Deploy it (zero-downtime deployment)

## Troubleshooting

### Build Fails

- Check build logs in Railway dashboard
- Ensure all NuGet packages are in `.csproj`
- Verify .NET 8.0 is specified in project file

### Application Won't Start

- Check environment variables are set correctly
- Verify Resend API key is valid
- Check application logs in Railway dashboard

### Database Issues

- Remember: Free tier has ephemeral storage
- Consider upgrading to Pro or using PostgreSQL
- Database file location: `/app/volunteers.db`

### Email Not Sending

- Verify `Resend__ApiKey` environment variable is set
- Check `Resend__FromEmail` uses verified domain
- Review application logs for email errors

## Railway Pricing

### Hobby Plan (Free Tier)
- $5 credit per month
- 500 hours of usage
- 512 MB RAM
- Ephemeral storage (data lost on restart)

### Pro Plan ($20/month)
- $20 credit per month
- Persistent volumes
- More resources
- Better for production

## Alternative: Railway PostgreSQL

Instead of SQLite, you can use Railway's PostgreSQL:

1. Add PostgreSQL service in Railway
2. Get connection string from PostgreSQL service
3. Update `Program.cs` to use PostgreSQL instead of SQLite
4. Update `VolunteerSignUpSystem.csproj` to include PostgreSQL package

## Support

- Railway Docs: [docs.railway.app](https://docs.railway.app)
- Railway Discord: [discord.gg/railway](https://discord.gg/railway)
- Check Railway dashboard logs for detailed error messages

