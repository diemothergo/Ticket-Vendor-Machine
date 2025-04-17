using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Configuration;
namespace Ticket_Vendor_Machine
{
    public partial class TicketForm : Form
    {
        private ComboBox cbDestination;
        private ComboBox cbPaymentMethod;
        private TextBox txtPaymentDetail;
        private Button btnBuyTicket;
        private DataGridView dgvTickets;
        private Label lblStatus;

        private string connStr = ConfigurationManager.ConnectionStrings["TicketSystemDB"].ConnectionString;

        private void InitializeComponent()
        {
            this.cbDestination = new ComboBox();
            this.cbPaymentMethod = new ComboBox();
            this.txtPaymentDetail = new TextBox();
            this.btnBuyTicket = new Button();
            this.dgvTickets = new DataGridView();
            this.lblStatus = new Label();

            this.Text = "Ticket Vendor Machine";
            this.Size = new System.Drawing.Size(600, 500);

            cbDestination.Location = new System.Drawing.Point(30, 30);
            cbDestination.Size = new System.Drawing.Size(200, 25);
            cbDestination.DropDownStyle = ComboBoxStyle.DropDownList;

            cbPaymentMethod.Location = new System.Drawing.Point(30, 70);
            cbPaymentMethod.Size = new System.Drawing.Size(200, 25);
            cbPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;

            txtPaymentDetail.Location = new System.Drawing.Point(30, 110);
            txtPaymentDetail.Size = new System.Drawing.Size(200, 25);
            txtPaymentDetail.ForeColor = Color.Gray;
            txtPaymentDetail.Text = "Nhập thông tin thanh toán...";
            txtPaymentDetail.GotFocus += (s, e) =>
            {
                if (txtPaymentDetail.Text == "Nhập thông tin thanh toán...")
                {
                    txtPaymentDetail.Text = "";
                    txtPaymentDetail.ForeColor = Color.Black;
                }
            };
            txtPaymentDetail.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtPaymentDetail.Text))
                {
                    txtPaymentDetail.Text = "Nhập thông tin thanh toán...";
                    txtPaymentDetail.ForeColor = Color.Gray;
                }
            };

            btnBuyTicket.Location = new System.Drawing.Point(30, 150);
            btnBuyTicket.Size = new System.Drawing.Size(100, 30);
            btnBuyTicket.Text = "Mua vé";
            btnBuyTicket.Click += new EventHandler(this.btnBuyTicket_Click);

            lblStatus.Location = new System.Drawing.Point(30, 190);
            lblStatus.Size = new System.Drawing.Size(400, 25);
            lblStatus.ForeColor = System.Drawing.Color.Green;

            dgvTickets.Location = new System.Drawing.Point(30, 230);
            dgvTickets.Size = new System.Drawing.Size(520, 200);
            dgvTickets.ReadOnly = true;
            dgvTickets.AllowUserToAddRows = false;
            dgvTickets.AllowUserToDeleteRows = false;

            this.Controls.Add(cbDestination);
            this.Controls.Add(cbPaymentMethod);
            this.Controls.Add(txtPaymentDetail);
            this.Controls.Add(btnBuyTicket);
            this.Controls.Add(lblStatus);
            this.Controls.Add(dgvTickets);

            this.Load += new EventHandler(this.TicketForm_Load);
        }

        private void TicketForm_Load(object sender, EventArgs e)
        {
            cbDestination.Items.AddRange(new string[] { "BenThanh", "SuoiTien", "TanSonNhat", "District7" });
            cbPaymentMethod.Items.AddRange(new string[] { "Credit Card", "Momo", "ZaloPay", "VNPay" });
            LoadTickets();
        }

        private void btnBuyTicket_Click(object sender, EventArgs e)
        {
            string destination = cbDestination.Text;
            string paymentMethod = cbPaymentMethod.Text;
            string paymentDetails = txtPaymentDetail.Text;

            if (string.IsNullOrEmpty(destination) || string.IsNullOrEmpty(paymentMethod) ||
                string.IsNullOrWhiteSpace(paymentDetails) || paymentDetails == "Nhập thông tin thanh toán...")
            {
                lblStatus.Text = "❌ Vui lòng điền đầy đủ thông tin.";
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    string query = "INSERT INTO Tickets (Destination, PaymentMethod, PaymentDetails) VALUES (@dest, @method, @details)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@dest", destination);
                    cmd.Parameters.AddWithValue("@method", paymentMethod);
                    cmd.Parameters.AddWithValue("@details", paymentDetails);
                    cmd.ExecuteNonQuery();
                    lblStatus.Text = "✅ Vé đã được mua thành công!";
                }
                LoadTickets();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ Lỗi: " + ex.Message;
            }
        }

        private void LoadTickets()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Tickets ORDER BY IssuedAt DESC", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvTickets.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "❌ Không thể tải danh sách vé: " + ex.Message;
            }
        }
    }
}
