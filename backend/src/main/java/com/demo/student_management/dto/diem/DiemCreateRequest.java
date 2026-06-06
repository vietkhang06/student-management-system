package com.demo.student_management.dto.diem;

import lombok.Data;

import java.math.BigDecimal;

@Data
public class DiemCreateRequest {
    private String idHocSinh;
    private String idMonHoc;
    private String idHocKy;
    private BigDecimal diem15;
    private BigDecimal diem45;
    private BigDecimal diemCk;
}