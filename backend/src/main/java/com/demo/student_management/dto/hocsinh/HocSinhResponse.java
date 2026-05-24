package com.demo.student_management.dto.hocsinh;

import lombok.AllArgsConstructor;
import lombok.Data;

import java.time.LocalDate;

@Data
@AllArgsConstructor
public class HocSinhResponse {
    private String idHocSinh;
    private String idLop;
    private String ten;
    private String gioiTinh;
    private LocalDate ngaySinh;
    private String diaChi;
    private String email;
}