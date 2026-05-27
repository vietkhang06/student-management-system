package com.demo.student_management.dto.admin;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class TeacherUpdateRequest {
    private String ten;
    private String matKhau; // optional, only updated if not empty/null
    private String gioiTinh;
    private String email;
    private String sdt;
    private String idLop;
    private String idMonHoc;
    private Boolean active;
}
