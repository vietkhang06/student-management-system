package com.demo.student_management.service.impl;

import com.demo.student_management.dto.baocao.BaoCaoHocKyResponse;
import com.demo.student_management.dto.baocao.BaoCaoMonResponse;
import com.demo.student_management.entity.ChiTietDiem;
import com.demo.student_management.entity.Lop;
import com.demo.student_management.entity.MonHoc;
import com.demo.student_management.exception.BusinessException;
import com.demo.student_management.repository.ChiTietDiemRepository;
import com.demo.student_management.repository.HocSinhRepository;
import com.demo.student_management.repository.LopRepository;
import com.demo.student_management.repository.MonHocRepository;
import com.demo.student_management.service.BaoCaoService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.math.BigDecimal;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class BaoCaoServiceImpl implements BaoCaoService {

    private final LopRepository lopRepository;

    private final MonHocRepository monHocRepository;

    private final ChiTietDiemRepository chiTietDiemRepository;

    private final HocSinhRepository hocSinhRepository;

    @Override
    public BaoCaoMonResponse baoCaoTongKetMon(
            String idLop,
            String idMonHoc,
            String idHocKy
    ) {

        Lop lop = lopRepository.findById(idLop)
                .orElseThrow(() ->
                        new BusinessException("Không tìm thấy lớp")
                );

        MonHoc monHoc = monHocRepository.findById(idMonHoc)
                .orElseThrow(() ->
                        new BusinessException("Không tìm thấy môn học")
                );

        List<ChiTietDiem> danhSach =
                chiTietDiemRepository
                        .findByHocSinh_Lop_IdLopAndMonHoc_IdMonHocAndHocKy_IdHocKy(
                                idLop,
                                idMonHoc,
                                idHocKy
                        );

        int tongHocSinh = danhSach.size();

        int soLuongDat = (int) danhSach.stream()
                .filter(x ->
                        x.getDiemTb() != null
                                && x.getDiemTb()
                                .compareTo(BigDecimal.valueOf(5)) >= 0
                )
                .count();

        double tyLeDat = tongHocSinh == 0
                ? 0
                : ((double) soLuongDat / tongHocSinh) * 100;

        return new BaoCaoMonResponse(
                lop.getIdLop(),
                lop.getTenLop(),
                monHoc.getIdMonHoc(),
                monHoc.getTenMonHoc(),
                idHocKy,
                tongHocSinh,
                soLuongDat,
                tyLeDat
        );
    }

    @Override
    public BaoCaoHocKyResponse baoCaoTongKetHocKy(
            String idLop,
            String idHocKy
    ) {

        Lop lop = lopRepository.findById(idLop)
                .orElseThrow(() ->
                        new BusinessException("Không tìm thấy lớp")
                );

        List<ChiTietDiem> danhSach =
                chiTietDiemRepository
                        .findByHocSinh_Lop_IdLopAndHocKy_IdHocKy(
                                idLop,
                                idHocKy
                        );

        int tongHocSinh =
                (int) hocSinhRepository.countByLop_IdLop(idLop);

        Map<String, List<ChiTietDiem>> groupedByStudent =
                danhSach.stream()
                        .collect(
                                Collectors.groupingBy(
                                        x -> x.getHocSinh().getIdHocSinh()
                                )
                        );

        int soLuongDat = 0;

        for (List<ChiTietDiem> scoresOfStudent
                : groupedByStudent.values()) {

            if (scoresOfStudent.isEmpty()) {
                continue;
            }

            double avg = scoresOfStudent.stream()
                    .filter(x -> x.getDiemTb() != null)
                    .mapToDouble(
                            x -> x.getDiemTb().doubleValue()
                    )
                    .average()
                    .orElse(0.0);

            if (avg >= 5.0) {
                soLuongDat++;
            }
        }

        double tyLeDat = tongHocSinh == 0
                ? 0
                : ((double) soLuongDat / tongHocSinh) * 100;

        return new BaoCaoHocKyResponse(
                lop.getIdLop(),
                lop.getTenLop(),
                idHocKy,
                tongHocSinh,
                soLuongDat,
                tyLeDat
        );
    }
}