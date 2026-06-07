package com.demo.student_management.service;

import com.demo.student_management.entity.LichSuHeThong;
import java.time.LocalDate;
import java.util.List;

public interface LichSuHeThongService {
    void log(String hanhDong, String chiTiet);
    List<LichSuHeThong> searchLogs(String keyword, String hanhDong, LocalDate tuNgay, LocalDate denNgay);
}
