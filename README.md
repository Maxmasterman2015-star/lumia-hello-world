# Lumia Hello World — UWP demo for the Lumia 950 XL

A complete, buildable **UWP "Hello World"** app targeting stock **Windows 10 Mobile (1709, build 10.0.15254)** on the Lumia 950 XL. It shows one page with a message and a **Tap me** button; each tap updates the text ("Hello from your Lumia 950 XL! (tapped N time(s))"). That's the whole demo — the goal is to prove your phone can still install and run your own code in 2026.

## What's in the package

```
LumiaHelloWorld/
├── .github/workflows/build.yml      # GitHub Actions: builds the ARM appxbundle in the cloud
├── .gitignore                       # keeps bin/obj/packages out of the repo
├── LumiaHelloWorld.sln              # Solution (open this in Visual Studio)
└── LumiaHelloWorld/
    ├── LumiaHelloWorld.csproj       # ARM (phone), ARM64, x86 build configs
    ├── packages.config              # NuGet: Microsoft.NETCore.UniversalWindowsPlatform 6.2.14
    ├── Package.appxmanifest         # Targets Windows.Universal, MaxVersionTested 10.0.15254.0
    ├── App.xaml / App.xaml.cs       # Standard UWP app entry point
    ├── MainPage.xaml / .cs          # The Hello World page + tap button
    ├── LumiaHelloWorld_TemporaryKey.pfx/.cer   # Self-signed test cert (CN=Lumia Dev)
    ├── Properties/
    │   ├── AssemblyInfo.cs
    │   └── Default.rd.xml           # .NET Native runtime directives
    └── Assets/                      # Tile logos + splash screen PNGs
```

## The five-minute version

1. **On a Windows 10/11 PC**: install Visual Studio 2019 (any edition, incl. free Community) with the **Universal Windows Platform development** workload and, in Individual Components, **Windows 10 SDK (10.0.17763.0)**.
2. **On the phone**: Settings → Update & security → For developers → **Developer mode** (this needs Wi‑Fi on first enable — it downloads a small developer package).
3. Connect both to the same Wi‑Fi network, then open `LumiaHelloWorld.sln`, pick **ARM** + **Device** (or **Remote Machine**, the phone's IP from Settings → System → About → Device information), and press **F5**.
4. The app installs and launches on the phone. Tap the button.

---

## Part 1 — Set up the phone (one-time)

1. **Charge it, sign in or skip.** If the phone has no Microsoft account, that's fine — sideloading doesn't need one.
2. **Enable Developer mode.** Settings → Update & security → For developers → select **Developer mode**. Accept the warning. First time, the phone needs internet (Wi‑Fi) to download the developer-mode package — if it fails, retry on a good network. Developer mode is what lets the phone install self-signed apps and accept remote deployments.
3. **Find the phone's IP address.** Settings → System → About → (Device information / More info). Write it down, e.g. `192.168.1.42`. Keep the phone on Wi‑Fi and the screen on for deployment.
4. **Optional but recommended: Device Portal + pin.** On the same For-developers screen, turn on **Device Portal** and **Device discovery**. You'll get a case-sensitive pairing PIN shown on screen. Visual Studio and WinAppDeployCmd will ask for this PIN on first connect.
5. **Pairing via a web browser.** Visit `https://<phone-ip>` in Edge/Chrome, accept the certificate warning, and enter the PIN when prompted. The Device Portal web UI is another way to install the appxbundle and the certificate (see Part 4).

## Part 2 — Build on a Windows PC (recommended path)

1. Install **Visual Studio 2019 Community** (free; search "Visual Studio 2019 Community download"). In the installer, tick:
   - Workload: **Universal Windows Platform development**
   - Individual components: **Windows 10 SDK (10.0.17763.0)** and **Windows 10 SDK (10.0.10240.0)** (optional, for min-version tooling)

   > **Why VS 2019 and not 2022?** VS 2022 can still *build* ARM32 UWP packages, but Visual Studio 2022 17.14 removed ARM32 .NET **debugging**, so F5-deploy-and-debug to the phone is broken there; VS 2019 (or VS 2022 ≤ 17.12) handles phone deployment out of the box.
2. Copy the `LumiaHelloWorld` folder to the PC, unzip if needed, and open `LumiaHelloWorld.sln`.
3. Right-click the solution → **Restore NuGet Packages** (restores `Microsoft.NETCore.UniversalWindowsPlatform` 6.2.14).
4. Platform selector → **ARM**. Target → **Device**. (When deploying over Wi‑Fi instead of USB, pick **Remote Machine** and enter the phone's IP + PIN.)
5. Press **F5**. First deploy installs the app on the phone and starts it — you'll see the splash screen, then the page with the button.

To produce an installable package instead: right-click project → **Store → Create App Packages...** → **Sideloading** → pick a location → tick **ARM** (the phone) → **Create**. This produces `LumiaHelloWorld_1.0.0.0_ARM_Test/LumiaHelloWorld_1.0.0.0_ARM_.appxbundle` plus a matching `.cer` in the same folder, then optionally auto-enrolls the test cert in your PC's Trusted People store.

## Part 3 — Deploy without Visual Studio (WinAppDeployCmd)

If you build the appxbundle once (Part 2 step 6) you can re-deploy to any phone from a plain command line. The tool ships with the Windows 10 SDK:

- Location on disk: `C:\Program Files (x86)\Windows Kits\10\bin\<SDK version>\x86\WinAppDeployCmd.exe`

```cmd
:: list devices on the network (time out after 3 s)
WinAppDeployCmd devices 3

:: install to the phone by IP (add -pin <PIN> on first connect)
WinAppDeployCmd install -file "LumiaHelloWorld_1.0.0.0_ARM_.appxbundle" -ip 192.168.1.42
```

Other handy forms: `update` instead of `install` keeps app data; `list -ip <ip>` shows installed packages; `uninstall -package LumiaHelloWorld_1.0.0.0_ARM__<hash> -ip <ip>` removes it. If you only have USB, enable Device Portal over USB — the phone appears in `WinAppDeployCmd devices` via the USB/IpOverUsb channel; alternatively use the Device Portal browser method below.

## Part 4 — Installing the certificate on the phone

Self-signed apps need their signing certificate trusted on the device. Two ways:

1. **Easiest — Device Portal (Wi‑Fi).** Browser → `https://<phone-ip>` → **Apps** manager page → **Deploy apps** → **Install Certificate** → upload `LumiaHelloWorld_TemporaryKey.cer`. Then upload the `.appxbundle` on the same page and click **Install** (you can also drag & drop).
2. **From the VS build output folder.** The folder `AppPackages\LumiaHelloWorld_1.0.0.0_ARM_Test` contains an `Add-AppxDevPackage.bat` that automates cert install + app install on a PC-connected device.

> The certificate is self-signed for testing (CN=Lumia Dev, valid to Nov 2031). If you ever get error `0x800B0109` / "trusting the certificate failed", the cert isn't installed on the phone — redo step 1.

## Part 5 — Building from your Mac (for now)

No Windows PC yet? Options, best first:

1. **Windows VM on the Mac (Intel Macs: free-ish; Apple Silicon: paid).**
   - Intel Mac + VMware Fusion / Parallels / VirtualBox: install Windows 10 x64, then VS 2019 as in Part 2. USB passthrough works for cabled deploys; Wi‑Fi deploy works if the VM is in bridged mode.
   - Apple Silicon (M1–M4): only Windows 11 **ARM64** runs. VS 2019 x86-on-ARM64 emulation is unreliable; use **VS 2022 ≤ 17.12** (which still supports building ARM32 packages — just not debugging them on ARM32 devices) and deploy via **WinAppDeployCmd over Wi‑Fi** (Part 3) or the Device Portal browser upload (Part 4). Or don't debug at all — build ARM `.appxbundle` in the VM, install via Device Portal.
2. **A spare/friend's Windows PC for the build**, then use your Mac for the Wi‑Fi deploy steps (WinAppDeployCmd and Device Portal are Windows-only too, but the Device Portal page is just a browser — on the Mac, open `https://<phone-ip>` and upload the appxbundle there).
3. **Cloud Windows VM** (Windows 365 / Azure VM with VS): same as option 1 but needs network reachability from VM to phone — usually only practical with the Device Portal upload route.
4. **GitHub Actions as builder** (no Windows machine at all) — the included workflow `.github/workflows/build.yml` does this for you. Push the project to GitHub; the workflow runs on `windows-2022` (which ships with the UWP workload, Windows SDK 10.0.17763 and the NuGet CLI), restores packages, builds the ARM `.appxbundle` signed with the checked-in test cert, and uploads it as a downloadable artifact. Concretely, from your Mac:

   ```bash
   cd LumiaHelloWorld                       # the folder containing LumiaHelloWorld.sln
   git init
   git add .
   git commit -m "Lumia Hello World UWP demo"
   # create an empty repo named e.g. lumia-hello-world on github.com first (no README/license,
   # or the push will be rejected), then:
   git remote add origin git@github.com:<your-username>/lumia-hello-world.git
   git branch -M main
   git push -u origin main
   ```

   On GitHub, open the repo → **Actions** tab → the **Build LumiaHelloWorld (ARM appxbundle)** workflow runs on your push (or via **Run workflow** for manual runs). When it finishes, open the run → scroll to **Artifacts** → download **LumiaHelloWorld-ARM-Test** (a zip with the `.appxbundle` and the matching `.cer`). Then install both on the phone via Device Portal (Part 4). GitHub-hosted runners are free for public repos; for a private repo you still get 2,000 free CI minutes/month, and this build takes ~3–5 of them.

> From the Mac you can still do the browser-based Device Portal install (option 2) — that's the zero-extra-software path: build the appxbundle anywhere (cloud/CI/friend's PC), then on the Mac visit the phone's Device Portal URL and upload the bundle + cert.

## Troubleshooting

| Symptom | Fix |
|---|---|
| Phone not found when deploying | Same Wi‑Fi network? Screen on? Device discovery enabled? Try `WinAppDeployCmd devices 3` from the PC |
| `DEP0001` / deployment error | Usually cert not trusted: install the `.cer` on the phone (Part 4), or re-deploy with VS which re-registers it |
| F5 works, app icon missing from list | Not an error — check All apps; tile icons come from Assets, all present |
| App starts then quits immediately | Almost always .NET Native Release build vs Debug build difference — use Debug first |
| VS "deploy failed" over USB | Use Wi‑Fi deploy (Remote Machine) instead; USB driver support for W10M phones is gone from modern Windows |
| `0x80073CFD` on install | Package architecture mismatch — make sure you built/deployed **ARM**, not x86 |

## Which parts you actually need

- **Have Windows + VS 2019?** → Part 1, Part 2, done.
- **Built the appxbundle, no VS on the deploy machine?** → Part 3 (WinAppDeployCmd) + Part 4 (cert).
- **On the Mac until the Windows PC arrives?** → Part 5, option 2 or 4 to build, then Part 4's Device Portal upload from any browser.

---

*Built for Lumia 950 XL · Windows 10 Mobile 1709 (10.0.15254) · UWP min 10.0.10240 · tested toolchain VS 2019 / SDK 10.0.17763*
