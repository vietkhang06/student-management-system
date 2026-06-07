package com.demo.student_management.controller;

import com.demo.student_management.entity.LichSuHeThong;
import com.demo.student_management.security.AuthorizationService;
import com.demo.student_management.service.LichSuHeThongService;
import lombok.RequiredArgsConstructor;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDate;
import java.util.List;

@RestController
@RequestMapping("/api/lichsu")
@RequiredArgsConstructor
public class LichSuHeThongController {

    private final LichSuHeThongService service;
    private final AuthorizationService authorizationService;

    @GetMapping("/search")
    public ResponseEntity<List<LichSuHeThong>> search(
            @RequestParam(required = false) String keyword,
            @RequestParam(required = false) String hanhDong,
            @RequestParam(required = false) @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate tuNgay,
            @RequestParam(required = false) @DateTimeFormat(iso = DateTimeFormat.ISO.DATE) LocalDate denNgay
    ) {
        authorizationService.requireAdmin();
        List<LichSuHeThong> logs = service.searchLogs(keyword, hanhDong, tuNgay, denNgay);
        return ResponseEntity.ok(logs);
    }
}
