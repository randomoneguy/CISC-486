# Quick Start: Testing Over Internet

## Easiest Method: VPN (No Port Forwarding)

### Step 1: Install VPN (2 minutes)
- Both players download **Radmin VPN** (free): https://www.radmin-vpn.com/
- Install and create account

### Step 2: Create Network (1 minute)
- Host: Create new network → Set name and password
- Client: Join network → Enter name and password

### Step 3: Connect (1 minute)
- Host: Click "Host" button → Note VPN IP (shown in console, e.g., `25.123.45.67`)
- Client: Enter VPN IP → Click "Client"
- Done! ✅

---

## Standard Method: Direct IP (Requires Port Forwarding)

### Host Setup:
1. Find public IP: https://whatismyipaddress.com/
2. Port forward UDP 7777 in router
3. Allow port 7777 in Windows Firewall
4. Click "Host" → Share public IP

### Client Setup:
1. Get host's public IP
2. Enter IP → Click "Client"
3. Done! ✅

---

## Same Network (Local):
- Host: Click "Host" → Share local IP (e.g., `192.168.1.100`)
- Client: Enter local IP → Click "Client"
- Done! ✅

---

## Troubleshooting

**Can't connect?**
- Try VPN method (easiest)
- Check firewall settings
- Verify IP address is correct
- Ensure host started first

**Still not working?**
- Test locally with `127.0.0.1` first
- Check both have same game version
- Verify NetworkManager is in scene

---

**Recommended: Use VPN method for easiest testing!**

