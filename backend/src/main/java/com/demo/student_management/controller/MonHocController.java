package com.demo.student_management.controller;

import com.demo.student_management.dto.monhoc.MonHocResponse;
import com.demo.student_management.service.MonHocService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;
import java.util.List;

@RestController
@RequestMapping("/api/mon-hoc")
@RequiredArgsConstructor
public class MonHocController {

    private final MonHocService monHocService;

    @GetMapping
    public List<MonHocResponse> getAll() {
        return monHocService.getAll();
    }
}
