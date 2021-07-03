using System;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Management;

namespace WindowsInfos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            // Set Button Settings, Explorer, Network
            // Add Features remove temp, cookies, etc.
            // run start windows
            // Create Wizzard installer

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point((((Screen.PrimaryScreen.Bounds.Width / 2 ) - this.Width)-115), (((Screen.PrimaryScreen.Bounds.Height / 2) - this.Height) +115));

            InitializeComponent();

            //Get Information
            GetUsername();
            GetWindowsVersion();
            GetCpuVersion();
            GetGpuVersion();
            GetMotherboardVersion();
            GetMemoryVersion();

            //Get Usage
            StartClockCpu();

            //Get Disk Info
            GetAllDisk();
        }
        public void GetWindowsVersion()
        {
            try
            {
                ManagementObjectSearcher myOperatingSystemObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                foreach (ManagementObject obj in myOperatingSystemObject.Get())
                {
                    lbl_WindowsVersionOsValue.Text = (obj["Caption"].ToString());
                    lbl_WindowsVersionBuildValue.Text = (obj["Version"].ToString());
                    lbl_WindowsVersionComputerNameValue.Text = (obj["CSName"].ToString());
                }
            }
            catch (Exception e) { MessageBox.Show(e.ToString()); }
        }
        public void GetCpuVersion()
        {
            try
            {
                ManagementObjectSearcher myCpuSystemObject = new ManagementObjectSearcher("select * from Win32_Processor");
                foreach (ManagementObject obj in myCpuSystemObject.Get())
                {
                    lbl_CpuVersion.Text = (obj["Name"].ToString());
                    lbl_CpuCore.Text = (obj["NumberOfCores"].ToString() + " / " + obj["ThreadCount"].ToString());
                }
            }
            catch(Exception e) { MessageBox.Show(e.ToString()); }
        }
        public void GetGpuVersion()
        {
            try
            {
                ManagementObjectSearcher myGpuSystemObject = new ManagementObjectSearcher("select * from Win32_VideoController");
                foreach (ManagementObject obj in myGpuSystemObject.Get())
                {
                    lbl_GpuVersion.Text = (obj["Name"].ToString());
                }
            }
            catch(Exception e) { MessageBox.Show(e.ToString()); }
        }
        public void GetMotherboardVersion()
        {
            try
            {
                ManagementObjectSearcher myMotherboardObject = new ManagementObjectSearcher("select * from Win32_BaseBoard");
                foreach (ManagementObject obj in myMotherboardObject.Get())
                {
                    lbl_MotherboardVersion.Text = (obj["Product"].ToString());
                }
            }
            catch(Exception e) { MessageBox.Show(e.ToString()); }
        }
        public void GetMemoryVersion()
        {
            try
            {
                UInt64 total = 0;
                ManagementObjectSearcher myMemoryObject = new ManagementObjectSearcher("select * from Win32_PhysicalMemory");
                foreach (ManagementObject obj in myMemoryObject.Get())
                {
                    total += (UInt64)obj.GetPropertyValue("Capacity");

                    lbl_MemoryVersion.Text = (obj["Manufacturer"].ToString()) + " " + (obj["PartNumber"].ToString()) + " " + total / 1073741824 + "GB";
                }
            }
            catch(Exception e) { MessageBox.Show(e.ToString()); }
        }
        public void SystemUsage(object sender, EventArgs e)
        {
            try
            {
                ManagementObjectSearcher cpuUsage = new ManagementObjectSearcher("select * from Win32_PerfFormattedData_PerfOS_Processor");
                foreach (ManagementObject obj in cpuUsage.Get())
                {
                    var usage = obj["PercentProcessorTime"];
                    lbl_CpuUsage.Text = (usage + " %");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }

            try
            {
                ManagementObjectSearcher memoryUsage = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
                foreach (ManagementObject obj in memoryUsage.Get())
                {
                    double free = Double.Parse(obj["FreePhysicalMemory"].ToString());
                    double total = Double.Parse(obj["TotalVisibleMemorySize"].ToString());
                    lbl_MemoryUsage.Text = Math.Round(((total - free) / total * 100), 2).ToString() + " %";
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }

            try
            {
                ManagementObjectSearcher networkSearcher = new ManagementObjectSearcher("select * from Win32_NetworkAdapter");
                foreach (ManagementObject obj in networkSearcher.Get())
                {
                    var adaptateur = obj["AdapterType"];

                    if (obj["NetConnectionStatus"] != null)
                    {
                        if (Convert.ToInt32(obj["NetConnectionStatus"]).Equals(0))
                        {
                            lbl_StatusNetwork.Text = "Disconnected";
                            lbl_NeworkAdaptateur.Text = adaptateur.ToString();
                            break;
                        }
                        else if (Convert.ToInt32(obj["NetConnectionStatus"]).Equals(1))
                        {
                            lbl_StatusNetwork.Text = "Connecting";
                            lbl_NeworkAdaptateur.Text = adaptateur.ToString();
                            break;
                        }
                        else if (Convert.ToInt32(obj["NetConnectionStatus"]).Equals(2))
                        {
                            lbl_StatusNetwork.Text = "Connected";
                            lbl_NeworkAdaptateur.Text = adaptateur.ToString();
                            break;
                        }
                        else
                        {
                            lbl_StatusNetwork.Text = "Hardware are not present or disabled or malfuntion";
                            lbl_NeworkAdaptateur.Text = adaptateur.ToString();
                            break;
                        }
                    } 
                }                
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }

            try
            {
                ManagementObjectSearcher ipv = new ManagementObjectSearcher("select * from Win32_NetworkAdapterConfiguration");
                foreach (ManagementObject obj in ipv.Get())
                {
                    string[] addresses = (string[])obj["IPAddress"];
                    var dhcpServer = obj["DHCPServer"];

                    if (addresses != null)
                    {
                        foreach (string ipaddress in addresses)
                        {
                            lbl_ipv6.Text = ("ipv6: " + ipaddress);
                            lbl_ipv4.Text = ("ipv4: " + ipaddress[0]);
                            lbl_DHCPServer.Text = ("DHCP Server : " + dhcpServer.ToString());
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        public void StartClockCpu()
        {
            try
            {
                Timer.Tick += SystemUsage;
                Timer.Start();
            }
            catch (Exception e) { MessageBox.Show(e.ToString()); }
        }
        public void GetAllDisk()
        {
            try
            {
                DriveInfo[] allDrives = DriveInfo.GetDrives();

                foreach (DriveInfo d in allDrives)
                {
                    string diskVolumeLabel = d.VolumeLabel;
                    string diskFormat = d.DriveFormat;
                    string driveLetter = d.Name;

                    int totalSize = (int)d.TotalSize;
                    int freeSpace = (int)d.TotalFreeSpace;

                    var totalKB = d.TotalSize / 1024;
                    var totalMB = totalKB / 1024;
                    var totalGB = totalMB / 1024;

                    var freeKB = d.TotalFreeSpace / 1024;
                    var freeMB = freeKB / 1024;
                    var freeGB = freeMB / 1024;

                    itemsList.SmallImageList = imageDisk;

                    ListViewItem _item = new ListViewItem();            

                    _item.ImageKey = "thisPC.png";
                    _item.SubItems.Add(diskVolumeLabel);
                    _item.SubItems.Add(driveLetter);
                    _item.SubItems.Add(diskFormat);
                    _item.SubItems.Add("Total " + totalGB.ToString() + " Go");
                    _item.SubItems.Add("Free " + freeGB.ToString() + " Go");

                    if (freeGB < (totalGB / 3))
                    {
                        _item.SubItems.Add("");
                        _item.SubItems[6].BackColor = System.Drawing.Color.Red;
                        _item.UseItemStyleForSubItems = false;
                    } else
                    {
                        _item.SubItems.Add("");
                        _item.SubItems[6].BackColor = System.Drawing.Color.Green;
                        _item.UseItemStyleForSubItems = false;
                    }

                    itemsList.Items.Add(_item);

                }
            }
            catch (Exception e) { MessageBox.Show(e.ToString()); }
        }
        private void itemsList_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo path = new System.Diagnostics.ProcessStartInfo();
                path.FileName = itemsList.SelectedItems[0].SubItems[2].Text;
                System.Diagnostics.Process.Start(Environment.GetEnvironmentVariable("WINDIR") + @"\explorer.exe", path.FileName.ToString());
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        private void GetUsername()
        {
            try
            {
                lbl_Username.Text = Environment.UserName;
                string userName = Environment.UserName;

                string profilePicture = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp\" + Environment.UserName + ".bmp";
                string profilePictureTemp = "C:\\ProgramData\\Microsoft\\User Account Pictures\\user-48.png";

                if (File.Exists(profilePicture))
                {
                    Pix.Image = Image.FromFile(profilePicture);
                }
                else
                {
                    Pix.Image = Image.FromFile(profilePictureTemp);
                }
            }
            catch(Exception e) { MessageBox.Show(e.ToString()); }
        }
        private void btn_Settings_Click(object sender, EventArgs e)
        {
            try
            {
                //ToDo Debug
                //System.Diagnostics.Process.Start("ms-settings:privacy-microphone");
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // -------------------------------------------------------------- //
        // ------------------------- Cleaner ---------------------------- //
        // -------------------------------------------------------------- //
    }
}
