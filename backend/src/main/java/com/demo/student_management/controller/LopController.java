package com.demo.student_management.controller;

import com.demo.student_management.dto.lop.LopDetailResponse;
import com.demo.student_management.dto.lop.LopResponse;
import com.demo.student_management.security.AuthorizationService;
import com.demo.student_management.service.LopService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/lop")
@RequiredArgsConstructor
public class LopController {

    private final LopService lopService;
    private final AuthorizationService authorizationService;

    @GetMapping
    public List<LopResponse> getAll() {
        List<LopResponse> lops = lopService.getAll();
        if (authorizationService.isBanQuanLy()) {
            return lops;
        }

        java.util.Set<String> assignedClassIds = authorizationService.getAssignedClassIdsForCurrentTeacher();
        return lops.stream()
                .filter(lop -> assignedClassIds.contains(lop.getIdLop()))
                .toList();
    }

    @GetMapping("/{idLop}")
    public LopDetailResponse getChiTietLop(@PathVariable String idLop) {
        authorizationService.requireCanAccessClass(idLop);
        return lopService.getChiTietLop(idLop);
    }
}
