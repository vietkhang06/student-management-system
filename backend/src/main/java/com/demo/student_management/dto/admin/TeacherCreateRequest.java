package com.demo.student_management.dto.admin;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class TeacherCreateRequest {
    private String idGiaoVien;
    private String ten;
    private String tenDangNhap;
    private String matKhau;
    private String gioiTinh;
    private String email;
    private String sdt;
    private String idLop;
    private String idLopChuNhiem;
    private String idMonHoc;
    private Boolean active;
}
