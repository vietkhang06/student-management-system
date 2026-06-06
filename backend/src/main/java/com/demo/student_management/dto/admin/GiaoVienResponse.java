package com.demo.student_management.dto.admin;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class GiaoVienResponse {
    private String idGiaoVien;
    private String tenGiaoVien;
    private String tenDangNhap;
    private String sdt;
    private String email;
    private String gioiTinh;
    private String idLop;
    private String tenLop;
    private String idLopChuNhiem;
    private String tenLopChuNhiem;
    private String idMonHoc;
    private String tenMonHoc;
    private Boolean active;
}
