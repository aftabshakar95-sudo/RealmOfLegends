# Deploy RealmOfLegends to Fly.io (100% FREE)

## ✅ What We Changed
- ✅ Switched from SQL Server to PostgreSQL (free on Fly.io)
- ✅ Updated NuGet package: `Npgsql.EntityFrameworkCore.PostgreSQL`
- ✅ Modified `Program.cs` to use PostgreSQL
- ✅ Created `fly.toml` configuration
- ✅ Updated Dockerfile for Fly.io

## 🚀 Deployment Steps

### 1. Install Fly CLI (One-time)
Open PowerShell and run:
```powershell
powershell -Command "iwr https://fly.io/install.ps1 -useb | iex"
```

**Close and reopen PowerShell after installation.**

### 2. Create Free Account
```powershell
fly auth signup
```
This opens your browser. Create account (no credit card required).

### 3. Login
```powershell
fly auth login
```

### 4. Navigate to Project
```powershell
cd c:\Users\hp\Desktop\RealmOfLegends
```

### 5. Create PostgreSQL Database (FREE)
```powershell
fly postgres create
```
When prompted:
- App name: `realmoflegends-db` (or any name)
- Region: Choose closest to you
- Configuration: **Development** (free tier)

**IMPORTANT**: Save the connection string it displays!

### 6. Attach Database to App
```powershell
fly postgres attach realmoflegends-db -a realmoflegends
```

### 7. Launch App (First Time)
```powershell
fly launch --no-deploy
```
When prompted:
- App name: `realmoflegends` (or choose your own)
- Region: Same as database
- Copy existing config: **Yes**

### 8. Set Connection String
The database attachment should auto-configure, but verify:
```powershell
fly secrets list
```

You should see `DATABASE_URL`. If not, set it:
```powershell
fly secrets set DATABASE_URL="your-postgres-connection-string"
```

### 9. Deploy!
```powershell
fly deploy
```

This will:
- Build your Docker image
- Push to Fly.io
- Start your app
- Run migrations
- Seed data

Takes 3-5 minutes.

### 10. Open Your Live Site!
```powershell
fly open
```

Your app will be live at: `https://realmoflegends.fly.dev`

## 📊 What You Get FREE

- ✅ 3 shared-cpu VMs (1GB RAM each)
- ✅ 160GB bandwidth/month
- ✅ PostgreSQL database (1GB)
- ✅ SSL certificate (HTTPS)
- ✅ Global CDN
- ✅ Auto-scaling
- ✅ Enough for 100-500 daily users

## 🔧 Useful Commands

**Check status:**
```powershell
fly status
```

**View logs:**
```powershell
fly logs
```

**SSH into machine:**
```powershell
fly ssh console
```

**Scale resources (if needed later):**
```powershell
fly scale memory 2048  # Upgrade to 2GB RAM
```

**Open dashboard:**
```powershell
fly dashboard
```

## 🐛 Troubleshooting

**If deployment fails:**

1. Check logs:
```powershell
fly logs
```

2. Verify database connection:
```powershell
fly postgres connect -a realmoflegends-db
```

3. Check secrets:
```powershell
fly secrets list
```

4. Retry deployment:
```powershell
fly deploy --no-cache
```

## 🔄 Future Updates

After making code changes:

1. Commit to git:
```powershell
git add .
git commit -m "Your changes"
git push
```

2. Deploy to Fly.io:
```powershell
fly deploy
```

That's it!

## 💰 Cost Monitoring

Check usage anytime:
```powershell
fly dashboard
```

The free tier should be enough. Fly.io will email you if you approach limits.

## 🎮 Your Live URLs

- **App**: https://realmoflegends.fly.dev
- **Dashboard**: https://fly.io/dashboard

---

**Need help?** Fly.io has excellent docs: https://fly.io/docs
