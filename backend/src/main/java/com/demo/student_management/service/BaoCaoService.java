package com.demo.student_management.service;

import com.demo.student_management.dto.baocao.BaoCaoMonResponse;
import com.demo.student_management.dto.baocao.BaoCaoHocKyResponse;

public interface BaoCaoService {

    BaoCaoMonResponse baoCaoTongKetMon(
            String idLop,
            String idMonHoc,
            String idHocKy
    );

    BaoCaoHocKyResponse baoCaoTongKetHocKy(
            String idLop,
            String idHocKy
    );
}