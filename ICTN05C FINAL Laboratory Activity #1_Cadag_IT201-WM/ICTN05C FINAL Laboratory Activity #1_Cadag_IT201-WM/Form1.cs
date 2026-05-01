using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ICTN05C_FINAL_Laboratory_Activity__1_Cadag_IT201_WM
{
    public partial class Form1 : Form
    {
        // Data structure for vehicle registration
        private class ParkingEntry
        {
            public string PlateNumber { get; set; }
            public string VehicleType { get; set; }
            public string Slot { get; set; }
            public DateTime EntryTime { get; set; }
        }

        // Store slot status and vehicle info
        private Dictionary<string, ParkingEntry> slotEntries = new Dictionary<string, ParkingEntry>();
        private Button selectedSlotButton = null;

        public Form1()
        {
            InitializeComponent();
            WireUpSlotButtons();
            InitializeVehicleTypes();
            InitializeDiscounts();
            WireUpActionButtons();
            SetTextBoxProperties();
        }

        private void SetTextBoxProperties()
        {
            // Read-only textboxes (except textBox6 and textBox12 for Hours Parked)
            textBox4.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox5.ReadOnly = true;
            textBox7.ReadOnly = true;
            textBox9.ReadOnly = true;
            textBox8.ReadOnly = true;
            textBox11.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox13.ReadOnly = true;
            // textBox12 (Registration Hours Parked) is editable
            // Numeric only
            textBox6.KeyPress += NumericTextBox_KeyPress;
            textBox1.KeyPress += NumericTextBox_KeyPress;
            textBox12.KeyPress += NumericTextBox_KeyPress;
        }

        private void NumericTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow only digits, control chars, and one dot
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // Wire up all slot buttons to a single handler
        private void WireUpSlotButtons()
        {
            foreach (var ctrl in groupBox3.Controls)
            {
                if (ctrl is Button btn && btn.Text.Length == 2)
                {
                    btn.Click += SlotButton_Click;
                }
            }
        }

        // Populate vehicle types
        private void InitializeVehicleTypes()
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(new string[] { "Car", "Sedan", "SUV", "Van", "Motorcycle" });
        }

        // Populate discounts
        private void InitializeDiscounts()
        {
            comboBox2.Items.Clear();
            comboBox2.Items.AddRange(new string[] { "None", "Senior (20%)", "Employee (20%)" });
            comboBox2.SelectedIndex = 0;
        }

        // Slot button click: select or retrieve
        private void SlotButton_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            // Extract slot name (first part before newline)
            string slotName = btn.Text.Split('\n')[0];
            if (btn.BackColor == Color.Lime) // Green: assign slot
            {
                textBox13.Text = slotName; // Assigned Slot in Registration
                selectedSlotButton = btn;
            }
            else if (btn.BackColor == Color.Red) // Red: retrieve info
            {
                if (slotEntries.TryGetValue(slotName, out var entry))
                {
                    // Populate Current Transaction
                    textBox4.Text = entry.PlateNumber;
                    textBox3.Text = entry.VehicleType;
                    textBox6.Text = ((int)(DateTime.Now - entry.EntryTime).TotalHours).ToString();
                    textBox5.Text = entry.Slot;
                    // Overtime fee will be calculated
                    CalculateFees();
                }
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button30_Click(object sender, EventArgs e)
        {

        }

        // --- Helper Methods ---
        private void ClearRegistrationFields()
        {
            textBox10.Text = ""; // Plate Number
            comboBox1.SelectedIndex = -1;
            textBox12.Text = ""; // Hours Parked
            textBox13.Text = ""; // Assigned Slot
            selectedSlotButton = null;
        }
        private void ClearTransactionFields()
        {
            textBox4.Text = "";
            textBox3.Text = "";
            textBox6.Text = "";
            textBox5.Text = "";
            textBox7.Text = "";
        }
        private void ClearFeeFields()
        {
            textBox9.Text = "";
            textBox8.Text = "";
            textBox11.Text = "";
        }
        private void ClearPaymentFields()
        {
            comboBox2.SelectedIndex = 0;
            textBox1.Text = "";
            textBox2.Text = "";
            richTextBox1.Text = "";
        }
        private void ClearAllFields()
        {
            ClearRegistrationFields();
            ClearTransactionFields();
            ClearFeeFields();
            ClearPaymentFields();
        }

        // --- Registration ---
        private void button4_Click(object sender, EventArgs e)
        {
            // Register Vehicle
            string plate = textBox10.Text.Trim();
            string vtype = comboBox1.SelectedItem?.ToString();
            string slot = textBox13.Text.Trim();
            if (string.IsNullOrEmpty(plate) || string.IsNullOrEmpty(vtype) || string.IsNullOrEmpty(slot))
            {
                MessageBox.Show("Please fill all registration fields and select a slot.");
                return;
            }
            if (slotEntries.ContainsKey(slot))
            {
                MessageBox.Show("Slot already occupied.");
                return;
            }
            // Store entry
            var entry = new ParkingEntry { PlateNumber = plate, VehicleType = vtype, Slot = slot, EntryTime = DateTime.Now };
            slotEntries[slot] = entry;
            // Mark slot as occupied and update button text
            var btn = groupBox3.Controls.OfType<Button>().FirstOrDefault(b => b.Text.StartsWith(slot));
            if (btn != null)
            {
                btn.BackColor = Color.Red;
                btn.Text = $"{slot}\n{plate}\n{vtype}";
            }
            // Clear registration fields
            ClearRegistrationFields();
        }

        // --- Update Status ---
        private void button5_Click(object sender, EventArgs e)
        {
            // Update current transaction info
            string slot = textBox5.Text.Trim();
            if (!slotEntries.ContainsKey(slot)) return;
            slotEntries[slot].PlateNumber = textBox4.Text.Trim();
            slotEntries[slot].VehicleType = textBox3.Text.Trim();
            // Update the slot button text as well
            var btn = groupBox3.Controls.OfType<Button>().FirstOrDefault(b => b.Text.StartsWith(slot));
            if (btn != null)
            {
                btn.Text = $"{slot}\n{textBox4.Text.Trim()}\n{textBox3.Text.Trim()}";
            }
        }

        // --- Remove Entry ---
        private void button6_Click(object sender, EventArgs e)
        {
            string slot = textBox5.Text.Trim();
            if (slotEntries.ContainsKey(slot))
            {
                slotEntries.Remove(slot);
                var btn = groupBox3.Controls.OfType<Button>().FirstOrDefault(b => b.Text.StartsWith(slot));
                if (btn != null)
                {
                    btn.BackColor = Color.Lime;
                    btn.Text = slot;
                }
                ClearTransactionFields();
            }
        }

        // --- Fee Calculation ---
        private decimal GetHourlyRate(string vtype)
        {
            switch (vtype)
            {
                case "Car": return 50;
                case "Sedan": return 50;
                case "SUV": return 70;
                case "Van": return 70;
                case "Motorcycle": return 30;
                default: return 0;
            }
        }
        private decimal GetServiceCharge() => 20;
        private decimal GetOvertimeFee(int hours, decimal hourlyRate)
        {
            if (hours > 8)
                return (hours - 8) * 10; // 10 per hour overtime after 8 hours
            return 0;
        }
        private decimal GetDiscountRate()
        {
            if (comboBox2.SelectedIndex == 1 || comboBox2.SelectedIndex == 2) return 0.2m; // Senior or Employee
            return 0m;
        }
        private void CalculateFees()
        {
            string vtype = textBox3.Text;
            int hours = 0;
            int.TryParse(textBox6.Text, out hours);
            decimal hourly = GetHourlyRate(vtype);
            decimal std = hourly * hours;
            decimal svc = GetServiceCharge();
            decimal ot = GetOvertimeFee(hours, hourly);
            decimal total = std + svc + ot;
            decimal discount = total * GetDiscountRate();
            total -= discount;
            textBox9.Text = std.ToString("F2"); // Standard Fee (per hour * hours)
            textBox8.Text = svc.ToString("F2"); // Service Charge
            textBox7.Text = ot.ToString("F2"); // Overtime Fee
            textBox11.Text = total.ToString("F2"); // Total
        }

        // --- Payment ---
        private void button3_Click(object sender, EventArgs e)
        {
            decimal total = 0, pay = 0;
            decimal.TryParse(textBox11.Text, out total);
            decimal.TryParse(textBox1.Text, out pay);
            if (pay < total)
            {
                MessageBox.Show("Insufficient payment.");
                return;
            }
            textBox2.Text = (pay - total).ToString("F2");
            // Reset slot
            string slot = textBox5.Text.Trim();
            if (slotEntries.ContainsKey(slot))
            {
                slotEntries.Remove(slot);
                var btn = groupBox3.Controls.OfType<Button>().FirstOrDefault(b => b.Text.StartsWith(slot));
                if (btn != null)
                {
                    btn.BackColor = Color.Lime;
                    btn.Text = slot;
                }
            }
            ClearTransactionFields();
            ClearFeeFields();
        }

        // --- Discount change ---
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateFees();
        }

        // --- Generate Receipt ---
        private void button2_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            sb.AppendLine("--- Parking Receipt ---");
            sb.AppendLine($"Plate: {textBox4.Text}");
            sb.AppendLine($"Vehicle: {textBox3.Text}");
            sb.AppendLine($"Slot: {textBox5.Text}");
            sb.AppendLine($"Duration: {textBox6.Text} hrs");
            sb.AppendLine($"Standard Fee: {textBox9.Text}");
            sb.AppendLine($"Service Charge: {textBox8.Text}");
            sb.AppendLine($"Overtime Fee: {textBox7.Text}");
            sb.AppendLine($"Total: {textBox11.Text}");
            sb.AppendLine($"Discount: {comboBox2.Text}");
            sb.AppendLine($"Paid: {textBox1.Text}");
            sb.AppendLine($"Change: {textBox2.Text}");
            richTextBox1.Text = sb.ToString();
        }

        // --- Clear Form ---
        private void button1_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        // --- Wire up events in constructor ---
        public void WireUpActionButtons()
        {
            button4.Click += button4_Click;
            button5.Click += button5_Click;
            button6.Click += button6_Click;
            button3.Click += button3_Click;
            button2.Click += button2_Click;
            button1.Click += button1_Click;
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            textBox6.TextChanged += textBox6_TextChanged;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            CalculateFees();
        }
    }
}
