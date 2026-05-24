package com.demo.student_management.controller;

import com.demo.student_management.dto.baocao.BaoCaoHocKyResponse;
import com.demo.student_management.dto.baocao.BaoCaoMonResponse;
import com.demo.student_management.security.AuthorizationService;
import com.demo.student_management.service.BaoCaoService;
import lombok.RequiredArgsConstructor;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/bao-cao")
@RequiredArgsConstructor
public class BaoCaoController {

    private final BaoCaoService baoCaoService;
    private final AuthorizationService authorizationService;

    @GetMapping("/tong-ket-mon")
    public BaoCaoMonResponse tongKetMon(
            @RequestParam String idLop,
            @RequestParam String idMonHoc,
            @RequestParam String idHocKy
    ) {

        authorizationService.requireCanViewScoreScope(idLop, idMonHoc, idHocKy);

        return baoCaoService.baoCaoTongKetMon(
                idLop,
                idMonHoc,
                idHocKy
        );
    }

    @GetMapping("/tong-ket-hoc-ky")
    public BaoCaoHocKyResponse tongKetHocKy(
            @RequestParam String idLop,
            @RequestParam String idHocKy
    ) {
        authorizationService.requireCanAccessClass(idLop);
        return baoCaoService.baoCaoTongKetHocKy(idLop, idHocKy);
    }
}
