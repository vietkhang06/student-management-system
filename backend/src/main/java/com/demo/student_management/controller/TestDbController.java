package com.demo.student_management.controller;

import com.demo.student_management.entity.Lop;
import com.demo.student_management.repository.LopRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;
import java.util.List;

@RestController
@RequiredArgsConstructor
public class TestDbController {

    private final JdbcTemplate jdbcTemplate;

    @GetMapping("/test-db")
    public List<String> testDb() {
        return jdbcTemplate.queryForList("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TAIKHOAN'", String.class);
    }
}