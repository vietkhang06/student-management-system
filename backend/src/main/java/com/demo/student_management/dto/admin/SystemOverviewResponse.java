package com.demo.student_management.dto.admin;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;
import java.util.List;

@Data
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class SystemOverviewResponse {
    private long totalStudents;
    private long totalClasses;
    private long totalTeachers;
    private long totalSubjects;
    private List<PhanCongResponse> assignments;
    private List<GiaoVienResponse> teachers;
}
