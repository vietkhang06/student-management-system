package com.demo.student_management.dto.auth;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;
import java.util.List;

@Data
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class ProfileResponse {
    private String idTaiKhoan;
    private String tenDangNhap;
    private String loaiTaiKhoan;
    private String ten;
    private String cmnd;
    private String sdt;
    private String ngaySinh;
    private String gioiTinh;
    private String chuNhiemLopId;
    private List<String> phanCongKeys;
}
