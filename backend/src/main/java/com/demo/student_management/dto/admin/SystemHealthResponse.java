package com.demo.student_management.dto.admin;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@Builder
@NoArgsConstructor
@AllArgsConstructor
public class SystemHealthResponse {
    private String databaseStatus;
    private String jvmVersion;
    private long totalMemoryBytes;
    private long freeMemoryBytes;
    private long usedMemoryBytes;
    private String osName;
}
