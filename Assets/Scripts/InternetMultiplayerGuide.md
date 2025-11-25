# Testing Multiplayer Over the Internet

This guide covers how to test your multiplayer game with another person over the internet using Unity Netcode for GameObjects.

## Method 1: Direct IP Connection (Recommended for Testing)

This is the standard method for testing multiplayer games over the internet.

### For the HOST (Person hosting the game):

1. **Find Your Public IP Address:**
   - Go to: https://whatismyipaddress.com/
   - Copy your IPv4 address (e.g., `123.45.67.89`)

2. **Configure Port Forwarding (if needed):**
   - Access your router's admin panel (usually `192.168.1.1` or `192.168.0.1`)
   - Find "Port Forwarding" or "Virtual Server" settings
   - Forward **Port 7777 (UDP)** to your computer's local IP address
   - Your local IP can be found by:
     - Windows: Open Command Prompt, type `ipconfig`, look for "IPv4 Address"
     - Usually something like `192.168.1.100`

3. **Configure Windows Firewall:**
   - Windows Defender Firewall → Advanced Settings
   - Inbound Rules → New Rule
   - Port → UDP → Specific local ports: `7777`
   - Allow the connection
   - Apply to all profiles

4. **Start the Host:**
   - Build your game or run in Unity Editor
   - Click "Host" button
   - Your IP address will be displayed on screen and in the console
   - Share your **public IP address** with the other person

### For the CLIENT (Person joining):

1. **Get the Host's IP Address:**
   - Ask the host for their public IP address

2. **Connect:**
   - Build your game or run in Unity Editor
   - Enter the host's IP address in the IP input field
   - Click "Client" button
   - Should connect!

---

## Method 2: Using a VPN (No Port Forwarding Needed)

If you can't port forward, use a VPN to create a virtual network.

### Recommended VPNs (Free):
- **Radmin VPN** (easiest, free)
- **Hamachi** (free, but limited)
- **ZeroTier** (free, open source)

### Setup Steps:

1. **Both players install the same VPN:**
   - Download and install (e.g., Radmin VPN)
   - Create an account

2. **Create/Join Network:**
   - Host creates a network
   - Client joins using network name and password
   - Both should see each other in the network

3. **Find VPN IP:**
   - Host finds their VPN IP address (shown in VPN software)
   - Usually something like `25.xxx.xxx.xxx`

4. **Connect:**
   - Client enters the host's VPN IP address
   - Click "Client" button
   - Should connect!

**Advantages:**
- No port forwarding needed
- Works behind any firewall
- Works with any NAT type
- Easy to set up

---

## Method 3: Same Network (Local Testing)

If both players are on the same WiFi/network:

1. **Host:**
   - Click "Host" button
   - Share your **local IP** (shown in console, e.g., `192.168.1.100`)

2. **Client:**
   - Enter host's local IP
   - Click "Client" button
   - Should connect immediately!

---

## Quick Comparison

| Method | Difficulty | Port Forwarding | Setup Time |
|--------|-----------|-----------------|------------|
| Direct IP | Easy | Required | 5-10 min |
| VPN | Very Easy | Not needed | 5 min |
| Same Network | Very Easy | Not needed | 1 min |

---

## UI Features

The NetworkManagerUI automatically:
- **Detects your local IP** when you start
- **Fetches your public IP** from the internet
- **Displays both IPs** when you click "Host"
- **Logs IPs to console** for easy copy-paste

### Optional UI Setup:

Add a TextMeshProUGUI element to display host IP:
1. Create a TextMeshProUGUI in your Canvas
2. Assign it to `hostIPText` in NetworkManagerUI component
3. It will automatically show IP when hosting

---

## Testing Checklist

### Before Testing:
- [ ] Both players have the same game build version
- [ ] NetworkManager is properly configured in scene
- [ ] Player prefabs have NetworkObject and NetworkTransform
- [ ] Spawn points are set up

### During Testing:
- [ ] Host starts first and waits
- [ ] Host shares IP address (or VPN IP)
- [ ] Client enters IP and connects
- [ ] Both players should see each other spawn
- [ ] Test movement, attacks, enemy interactions

---

## Troubleshooting

### "Connection Failed" or "Connection Timeout"

**Host Issues:**
- Check firewall settings (allow UDP port 7777)
- Verify port forwarding is correct
- Ensure router allows incoming connections
- Try disabling firewall temporarily to test

**Client Issues:**
- Verify IP address is correct (public IP, not 192.168.x.x)
- Check you're using the right IP (public vs VPN vs local)
- Ensure host has started before you try to connect
- Try disabling firewall temporarily

**Both:**
- Make sure both have same game version
- Check internet connection
- Verify NetworkManager is in the scene

### "Players don't see each other"

- Check NetworkPlayerSpawner is in scene
- Verify spawn points are assigned
- Check NetworkManager prefab list includes player prefab
- Ensure players have NetworkObject component

### "Enemies not moving"

- Enemies only run AI on server (host)
- Check EnemyAI has `IsServer` checks
- Verify enemies have NetworkObject component
- Check enemies are spawned correctly

### "High Latency/Lag"

- Normal for internet connections
- Check both players' internet speeds
- Try VPN method for better routing
- Consider geographic distance

---

## Security Note

When hosting with direct IP:
- Your public IP will be visible to the client
- Only share IP with trusted friends
- Consider using VPN for additional privacy
- Don't share your IP publicly

---

## Quick Start Guide

### Fastest Method (VPN):

1. **Both install Radmin VPN** (free)
2. **Host creates network** → Shares network name/password
3. **Client joins network**
4. **Host clicks "Host"** → Gets VPN IP (e.g., `25.123.45.67`)
5. **Client enters VPN IP** → Clicks "Client"
6. **Both connect!**

### Standard Method (Direct IP):

1. **Host finds public IP** (whatismyipaddress.com)
2. **Host forwards port 7777** in router
3. **Host allows port 7777** in firewall
4. **Host clicks "Host"** → Shares public IP
5. **Client enters public IP** → Clicks "Client"
6. **Both connect!**

---

## Pro Tips

1. **Test locally first:** Use `127.0.0.1` to ensure game works
2. **Use Discord/Skype:** Share IP through voice chat for faster testing
3. **Check router logs:** Some routers show connection attempts
4. **Try different ports:** If 7777 doesn't work, try 7778, 7779, etc.
5. **Public IP changes:** If you restart router, IP may change
6. **VPN is easiest:** For testing, VPN method is usually simplest

---

## Next Steps

Once basic connection works:
- Test with multiple friends
- Add connection approval (prevent unauthorized joins)
- Implement player authentication
- Add lobby/matchmaking system
- Consider dedicated server for better performance

---

That's it! You should be able to play together over the internet now. The VPN method is recommended for easiest setup without port forwarding.

