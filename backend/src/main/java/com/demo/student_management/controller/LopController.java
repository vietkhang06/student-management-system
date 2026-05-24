package com.demo.student_management.controller;

import com.demo.student_management.dto.lop.LopDetailResponse;
import com.demo.student_management.dto.lop.LopResponse;
import com.demo.student_management.service.LopService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/lop")
@RequiredArgsConstructor
public class LopController {

    private final LopService lopService;

    @GetMapping
    public List<LopResponse> getAll() {
        return lopService.getAll();
    }

    @GetMapping("/{idLop}")
    public LopDetailResponse getChiTietLop(@PathVariable String idLop) {
        return lopService.getChiTietLop(idLop);
    }
}