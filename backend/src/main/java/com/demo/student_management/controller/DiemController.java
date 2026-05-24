package com.demo.student_management.controller;

import com.demo.student_management.dto.diem.DiemCreateRequest;
import com.demo.student_management.dto.diem.DiemResponse;
import com.demo.student_management.security.AuthorizationService;
import com.demo.student_management.service.DiemService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/diem")
@RequiredArgsConstructor
public class DiemController {

    private final DiemService diemService;
    private final AuthorizationService authorizationService;

    @PostMapping
    public DiemResponse save(@RequestBody DiemCreateRequest request) {
        authorizationService.requireCanAccessStudentScore(request.getIdHocSinh(), request.getIdMonHoc(), request.getIdHocKy());
        return diemService.save(request);
    }

    @GetMapping
    public List<DiemResponse> getByClassSubjectTerm(
            @RequestParam String idLop,
            @RequestParam String idMonHoc,
            @RequestParam String idHocKy
    ) {
        authorizationService.requireCanViewScoreScope(idLop, idMonHoc, idHocKy);
        return diemService.getByClassSubjectTerm(idLop, idMonHoc, idHocKy);
    }
}
