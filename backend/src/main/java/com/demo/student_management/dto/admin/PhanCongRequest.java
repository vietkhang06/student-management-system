package com.demo.student_management.dto.admin;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class PhanCongRequest {
    private String idGiaoVien;
    private String idLop;
    private String idMonHoc;
    private String idHocKy;
}
