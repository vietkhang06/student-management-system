package com.demo.student_management.dto.hocsinh;

import lombok.Data;
import java.time.LocalDate;

@Data
public class HocSinhUpdateRequest {
    private String idLop;
    private String ten;
    private String gioiTinh;
    private LocalDate ngaySinh;
    private String diaChi;
    private String email;
}
