package com.demo.student_management.dto.baocao;

import lombok.AllArgsConstructor;
import lombok.Data;

@Data
@AllArgsConstructor
public class BaoCaoMonResponse {

    private String idLop;
    private String tenLop;

    private String idMonHoc;
    private String tenMonHoc;

    private String idHocKy;

    private Integer tongHocSinh;
    private Integer soLuongDat;

    private Double tyLeDat;
}