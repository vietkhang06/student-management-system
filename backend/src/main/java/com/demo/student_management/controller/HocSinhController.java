package com.demo.student_management.controller;

import com.demo.student_management.dto.hocsinh.HocSinhCreateRequest;
import com.demo.student_management.dto.hocsinh.HocSinhResponse;
import com.demo.student_management.service.HocSinhService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;
import com.demo.student_management.dto.hocsinh.HocSinhSummaryResponse;

import java.util.List;

@RestController
@RequestMapping("/api/hoc-sinh")
@RequiredArgsConstructor
public class HocSinhController {

    private final HocSinhService hocSinhService;

    @PostMapping
    public HocSinhResponse create(@RequestBody HocSinhCreateRequest request) {
        return hocSinhService.create(request);
    }

    @GetMapping
    public List<HocSinhResponse> getAll() {
        return hocSinhService.getAll();
    }

    @GetMapping("/{idHocSinh}")
    public HocSinhResponse getById(@PathVariable String idHocSinh) {
        return hocSinhService.getById(idHocSinh);
    }

    @GetMapping("/search")
    public List<HocSinhResponse> search(@RequestParam String ten) {
        return hocSinhService.searchByName(ten);
    }

    @GetMapping("/summary")
    public List<HocSinhSummaryResponse> getStudentSummaries() {
        return hocSinhService.getStudentSummaries();
    }
}