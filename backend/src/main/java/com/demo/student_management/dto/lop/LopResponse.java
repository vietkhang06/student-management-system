package com.demo.student_management.dto.lop;

import lombok.AllArgsConstructor;
import lombok.Data;

@Data
@AllArgsConstructor
public class LopResponse {
    private String idLop;
    private String tenLop;
    private Integer siSo;
}