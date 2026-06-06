package com.demo.student_management.dto.diem;

import lombok.AllArgsConstructor;
import lombok.Data;

import java.math.BigDecimal;

@Data
@AllArgsConstructor
public class DiemResponse {

    private String idHocSinh;

    private String tenHocSinh;

    private String idMonHoc;

    private String idHocKy;

    private BigDecimal diem15;

    private BigDecimal diem45;

    private BigDecimal diemCk;

    private BigDecimal diemTb;
}