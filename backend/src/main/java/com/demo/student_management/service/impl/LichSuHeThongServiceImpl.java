package com.demo.student_management.service.impl;

import com.demo.student_management.entity.LichSuHeThong;
import com.demo.student_management.repository.LichSuHeThongRepository;
import com.demo.student_management.security.AuthorizationService;
import com.demo.student_management.security.AuthenticatedUser;
import com.demo.student_management.service.LichSuHeThongService;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.time.LocalTime;
import java.util.List;

@Service
@RequiredArgsConstructor
public class LichSuHeThongServiceImpl implements LichSuHeThongService {

    private final LichSuHeThongRepository repository;
    private final AuthorizationService authorizationService;

    @Override
    @Transactional
    public void log(String hanhDong, String chiTiet) {
        String username = authorizationService.currentUser()
                .map(AuthenticatedUser::tenDangNhap)
                .orElse("Hệ thống");

        LichSuHeThong logEntry = LichSuHeThong.builder()
                .thoiGian(LocalDateTime.now())
                .nguoiThucHien(username)
                .hanhDong(hanhDong)
                .chiTiet(chiTiet)
                .build();

        repository.save(logEntry);
    }

    @Override
    @Transactional(readOnly = true)
    public List<LichSuHeThong> searchLogs(String keyword, String hanhDong, LocalDate tuNgay, LocalDate denNgay) {
        LocalDateTime start = tuNgay != null ? tuNgay.atStartOfDay() : null;
        LocalDateTime end = denNgay != null ? denNgay.atTime(LocalTime.MAX) : null;
        return repository.searchLogs(keyword, hanhDong, start, end);
    }
}
