package com.demo.student_management.controller;

import com.demo.student_management.dto.diem.DiemCreateRequest;
import com.demo.student_management.dto.diem.DiemResponse;
import com.demo.student_management.service.DiemService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/diem")
@RequiredArgsConstructor
public class DiemController {

    private final DiemService diemService;

    @PostMapping
    public DiemResponse save(@RequestBody DiemCreateRequest request) {
        return diemService.save(request);
    }

    @GetMapping
    public List<DiemResponse> getByClassSubjectTerm(
            @RequestParam String idLop,
            @RequestParam String idMonHoc,
            @RequestParam String idHocKy
    ) {
        return diemService.getByClassSubjectTerm(idLop, idMonHoc, idHocKy);
    }
}