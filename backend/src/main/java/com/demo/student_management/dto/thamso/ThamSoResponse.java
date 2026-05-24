package com.demo.student_management.dto.thamso;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class ThamSoResponse {
    private String idThamSo;
    private String tenThamSo;
    private Integer kieuThamSo;
    private String giaTriThamSo;
}
