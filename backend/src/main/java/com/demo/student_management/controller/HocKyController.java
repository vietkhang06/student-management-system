package com.demo.student_management.controller;

import com.demo.student_management.dto.hocky.HocKyResponse;
import com.demo.student_management.service.HocKyService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;
import java.util.List;

@RestController
@RequestMapping("/api/hoc-ky")
@RequiredArgsConstructor
public class HocKyController {

    private final HocKyService hocKyService;

    @GetMapping
    public List<HocKyResponse> getAll() {
        return hocKyService.getAll();
    }
}
