package com.demo.student_management.dto.hocsinh;

import lombok.AllArgsConstructor;
import lombok.Data;

@Data
@AllArgsConstructor
public class HocSinhSummaryResponse {

    private String idHocSinh;

    private String ten;

    private String idLop;

    private Double tbHocKy1;

    private Double tbHocKy2;
}