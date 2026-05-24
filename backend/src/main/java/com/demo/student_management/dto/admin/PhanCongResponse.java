package com.demo.student_management.dto.admin;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class PhanCongResponse {
    private String idPhanCong;
    private String idGiaoVien;
    private String tenGiaoVien;
    private String idLop;
    private String tenLop;
    private String idMonHoc;
    private String tenMonHoc;
    private String idHocKy;
    private String tenHocKy;
}
