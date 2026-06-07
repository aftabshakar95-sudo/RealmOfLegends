# Deploy RealmOfLegends to Render.com (100% FREE - No Credit Card!)

## ✅ Why Render.com?

- ✅ **100% FREE** tier (no credit card required!)
- ✅ Full ASP.NET Core support via Docker
- ✅ Free PostgreSQL database included
- ✅ 750 hours/month (enough for 24/7 uptime)
- ✅ SSL certificates included
- ✅ Deploy directly from GitHub
- ✅ Auto-deploy on git push

## 🚀 Deployment Steps (10 Minutes)

### Step 1: Create Free Render Account

1. Go to: **https://render.com**
2. Click **"Get Started for Free"**
3. Sign up with GitHub (recommended) or email
4. **No credit card required!**

### Step 2: Connect GitHub Repository

1. After login, go to: **https://dashboard.render.com**
2. Click **"New +"** → **"Web Service"**
3. Click **"Connect GitHub"** (if not already connected)
4. Select your repository: **RealmOfLegends**
5. Click **"Connect"**

### Step 3: Configure Web Service

**Basic Settings:**
- **Name**: `realmoflegends` (or your choice)
- **Region**: `Oregon (US West)` (free tier)
- **Branch**: `main`
- **Root Directory**: Leave blank
- **Runtime**: `Docker`

**Build & Deploy:**
- **Dockerfile Path**: `./Dockerfile`

**Instance Type:**
- Select: **Free** ($0/month)

**Advanced Settings (Expand):**
- **Auto-Deploy**: `Yes` (deploys automatically on git push)

Click **"Create Web Service"** (don't deploy yet - we need database first!)

### Step 4: Create PostgreSQL Database

1. From dashboard, click **"New +"** → **"PostgreSQL"**
2. **Name**: `realmoflegends-postgres`
3. **Database**: `realmoflegends`
4. **User**: `realmoflegends`
5. **Region**: `Oregon (US West)` (same as web service)
6. **Plan**: **Free** ($0/month)
7. Click **"Create Database"**

Wait 1-2 minutes for database to initialize.

### Step 5: Connect Database to Web Service

1. Go to your **Web Service** dashboard
2. Click **"Environment"** tab
3. Click **"Add Environment Variable"**
4. Add this variable:
   - **Key**: `ConnectionStrings__DefaultConnection`
   - **Value**: Copy the **Internal Database URL** from your PostgreSQL dashboard

**To get Internal Database URL:**
1. Open your PostgreSQL database dashboard
2. Find **"Internal Database URL"** (starts with `postgresql://`)
3. Copy the entire URL
4. Paste it as the value

### Step 6: Add Additional Environment Variables

Add these in the Environment tab:

```
ASPNETCORE_URLS = http://0.0.0.0:8080
ASPNETCORE_ENVIRONMENT = Production
```

### Step 7: Deploy!

1. Click **"Manual Deploy"** → **"Deploy latest commit"**
2. Watch the logs (takes 5-10 minutes first time)
3. Build progress will show in real-time

### Step 8: Your Site is Live!

Once deployment succeeds:
- Your URL: `https://realmoflegends.onrender.com`
- Click the URL in Render dashboard to open

## 🎮 Access Your Application

**Main URLs:**
- Home: `https://realmoflegends.onrender.com`
- Login: `https://realmoflegends.onrender.com/Account/Login`
- Register: `https://realmoflegends.onrender.com/Account/Register`

**First-time setup:**
1. Register a new account
2. Create your character
3. Start playing!

## 💰 Free Tier Limits

**Web Service (Free):**
- 750 hours/month (31 days × 24 hours = 744 hours)
- Spins down after 15 minutes of inactivity
- Spins back up automatically when accessed (takes ~30 seconds)
- 512 MB RAM
- 0.1 CPU

**PostgreSQL (Free):**
- 1 GB storage
- Expires after 90 days (but you can create a new one)
- Automatic backups

**More than enough for:**
- Personal projects
- Portfolio demos
- Testing
- 100-500 daily users

## 🔧 Useful Commands & Tips

**View Logs:**
- Go to your service dashboard
- Click **"Logs"** tab
- Real-time logs show all activity

**Restart Service:**
- Go to **"Settings"** tab
- Click **"Manual Deploy"** → **"Clear build cache & deploy"**

**Update Code:**
Just push to GitHub:
```powershell
git add .
git commit -m "Your changes"
git push origin main
```
Render auto-deploys!

**Custom Domain (Optional - Free):**
1. Go to **"Settings"** tab
2. Click **"Add Custom Domain"**
3. Follow DNS instructions

**Health Check:**
- Render automatically pings your site every 5 minutes
- Keeps it awake if you're actively using it

## 🐛 Troubleshooting

### Build fails?
1. Check logs for errors
2. Verify Dockerfile exists in repo
3. Try **"Clear build cache & deploy"**

### Database connection fails?
1. Verify `ConnectionStrings__DefaultConnection` is set
2. Use **Internal Database URL** (not external)
3. Make sure PostgreSQL database is **Available** (green status)

### Site is slow on first load?
- Free tier spins down after 15 min of inactivity
- First request takes ~30 seconds to spin up
- After that, it's fast!

### Keep site awake (Optional):
Use a free uptime monitor:
- **UptimeRobot** (https://uptimerobot.com)
- Ping your site every 5 minutes
- Keeps it from spinning down

## 🔄 Future Updates

**Automatic Deploy:**
Already configured! Just push to GitHub:
```powershell
git add .
git commit -m "Update game"
git push
```

**Manual Deploy:**
1. Go to Render dashboard
2. Click **"Manual Deploy"**
3. Select **"Deploy latest commit"**

## 📊 Monitoring

**Dashboard Shows:**
- Deploy history
- Build logs
- Service metrics
- Uptime
- Request logs

**Access it at:**
https://dashboard.render.com

## 🎯 Advantages of Render.com

✅ No credit card required
✅ Simple setup (no CLI needed)
✅ Auto-deploy from GitHub
✅ Free SSL certificates
✅ Great for portfolios
✅ Easy scaling when needed
✅ Good free tier limits

## 🆚 vs Your Current Host

| Feature | Current Host | Render.com |
|---------|--------------|------------|
| Multi-project .NET | ❌ Broken | ✅ Works perfectly |
| Cost | ❓ | ✅ $0 |
| Setup | ❌ Impossible | ✅ 10 minutes |
| Auto-deploy | ❌ No | ✅ Yes |
| SSL | ❓ | ✅ Free |
| Database | ❓ | ✅ Free PostgreSQL |

## 📞 Support

- Docs: https://render.com/docs
- Community: https://community.render.com
- Status: https://status.render.com

---

**That's it!** Your game will be live at `https://realmoflegends.onrender.com` in 10 minutes! 🎮🚀
