using De01.Model;
using System;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace De01
{
    public partial class frmSinhvien : Form
    {
        public frmSinhvien()
        {
            InitializeComponent();
        }

        private Model1 dbContext;

        private void frmSinhvien_Load(object sender, EventArgs e)
        {
            dbContext = new Model1();
            LoadDanhSachSinhvien();
            LoadDanhSachLop();
        }

        private void LoadDanhSachSinhvien()
        {
            var sinhvienList = dbContext.SinhViens.Include(sv => sv.Lop).ToList();
            lvSinhVien.Items.Clear();
            foreach (var sv in sinhvienList)
            {
                ListViewItem item = new ListViewItem(sv.MaSV);
                item.SubItems.Add(sv.HotenSV);
                item.SubItems.Add(sv.NgaySinh.HasValue ? sv.NgaySinh.Value.ToString("dd/MM/yyyy") : string.Empty);
                item.SubItems.Add(sv.Lop?.TenLop ?? "");
                lvSinhVien.Items.Add(item);
            }
        }

        private void LoadDanhSachLop()
        {
            var lopList = dbContext.Lops.ToList();
            cboLopHoc.Items.Clear();
            foreach (var lop in lopList)
            {
                cboLopHoc.Items.Add(lop.TenLop);
            }
        }

        private void btLuu_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn lưu thông tin này không?",
                "Xác nhận lưu",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                var sinhvien = dbContext.SinhViens.Find(txtMaSV.Text);
                if (sinhvien == null)
                {
                    var selectedLop = dbContext.Lops.FirstOrDefault(l => l.TenLop == cboLopHoc.SelectedItem.ToString());
                    sinhvien = new SinhVien
                    {
                        MaSV = txtMaSV.Text,
                        HotenSV = txtHoTen.Text,
                        NgaySinh = dtpNgaySinh.Value,
                        MaLop = selectedLop?.MaLop
                    };

                    dbContext.SinhViens.Add(sinhvien);
                    dbContext.SaveChanges();
                    LoadDanhSachSinhvien();
                    ClearForm();
                }
                
            }
            // Nếu người dùng chọn "No", không làm gì và không lưu dữ liệu
        }


        private void btSua_Click(object sender, EventArgs e)
        {
            var sinhvien = dbContext.SinhViens.FirstOrDefault(sv => sv.MaSV == txtMaSV.Text);
            if (sinhvien != null)
            {
                var selectedLop = dbContext.Lops.FirstOrDefault(l => l.TenLop == cboLopHoc.SelectedItem.ToString());

                sinhvien.HotenSV = txtHoTen.Text;
                sinhvien.NgaySinh = dtpNgaySinh.Value;
                sinhvien.MaLop = selectedLop.MaLop;
                sinhvien.Lop = selectedLop;

                dbContext.SaveChanges();
                LoadDanhSachSinhvien();
                ClearForm();
            }
        }

        private void btXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn xóa sinh viên này không?", "Xác nhận xóa", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var sinhvien = dbContext.SinhViens.FirstOrDefault(sv => sv.MaSV == txtMaSV.Text);
                if (sinhvien != null)
                {
                    dbContext.SinhViens.Remove(sinhvien);
                    dbContext.SaveChanges();
                    LoadDanhSachSinhvien();
                    ClearForm();
                }
            }
        }

        private void btTim_Click(object sender, EventArgs e)
        {
            var searchTerm = txtTimKiem.Text.ToLower();
            var filteredList = dbContext.SinhViens
                                .Include(sv => sv.Lop)
                                .Where(sv => sv.HotenSV.ToLower().Contains(searchTerm))
                                .ToList();

            lvSinhVien.Items.Clear();
            foreach (var sv in filteredList)
            {
                ListViewItem item = new ListViewItem(sv.MaSV);
                item.SubItems.Add(sv.HotenSV);
                item.SubItems.Add(sv.NgaySinh.HasValue ? sv.NgaySinh.Value.ToString("dd/MM/yyyy") : string.Empty);
                item.SubItems.Add(sv.Lop?.TenLop ?? "");
                lvSinhVien.Items.Add(item);
            }
        }

        private void btThem_Click(object sender, EventArgs e)
        {
            // Validate input fields
            if (string.IsNullOrWhiteSpace(txtMaSV.Text) ||
                string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                cboLopHoc.SelectedItem == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Create a new SinhVien object
            var selectedLop = dbContext.Lops.FirstOrDefault(l => l.TenLop == cboLopHoc.SelectedItem.ToString());
            var sinhVien = new SinhVien
            {
                MaSV = txtMaSV.Text,
                HotenSV = txtHoTen.Text,
                NgaySinh = dtpNgaySinh.Value,
                MaLop = selectedLop?.MaLop
            };

            // Check if the student already exists
            var existingSinhVien = dbContext.SinhViens.FirstOrDefault(sv => sv.MaSV == sinhVien.MaSV);
            if (existingSinhVien != null)
            {
                MessageBox.Show("Mã sinh viên này đã tồn tại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Add the new SinhVien to the database and save changes
            dbContext.SinhViens.Add(sinhVien);
            dbContext.SaveChanges();

            // Reload the data to reflect the new addition
            LoadDanhSachSinhvien();
            ClearForm();
        }


        private void btKhong_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void lvSinhvien_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSinhVien.SelectedItems.Count > 0)
            {
                var selectedItem = lvSinhVien.SelectedItems[0];
                txtMaSV.Text = selectedItem.SubItems[0].Text;
                txtHoTen.Text = selectedItem.SubItems[1].Text;
                if (DateTime.TryParseExact(selectedItem.SubItems[2].Text, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime ngaysinh))
                {
                    dtpNgaySinh.Value = ngaysinh;
                }
                else
                {
                    dtpNgaySinh.Value = DateTime.Now;
                }
                cboLopHoc.SelectedItem = selectedItem.SubItems[3].Text;
            }
        }

        private void ClearForm()
        {
            txtMaSV.Clear();
            txtHoTen.Clear();
            dtpNgaySinh.Value = DateTime.Now;
            cboLopHoc.SelectedIndex = -1;
        }

        private void btThoat_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
        "Bạn có chắc chắn muốn đóng ứng dụng không?",
        "Xác nhận thoát",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Nếu người dùng chọn "Yes", đóng ứng dụng
                this.Close();
            }
            // Nếu người dùng chọn "No", không làm gì, giữ cửa sổ mở
        }
    }
}
