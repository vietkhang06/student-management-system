package com.demo.student_management.dto.baocao;

import lombok.AllArgsConstructor;
import lombok.Data;

@Data
@AllArgsConstructor
public class BaoCaoHocKyResponse {
    private String idLop;
    private String tenLop;
    private String idHocKy;
    private Integer tongHocSinh;
    private Integer soLuongDat;
    private Double tyLeDat;
}