package com.demo.student_management.repository;

import com.demo.student_management.entity.LichSuHeThong;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;

import java.time.LocalDateTime;
import java.util.List;

public interface LichSuHeThongRepository extends JpaRepository<LichSuHeThong, Long> {

    @Query("SELECT l FROM LichSuHeThong l WHERE " +
            "(:keyword IS NULL OR :keyword = '' OR LOWER(l.nguoiThucHien) LIKE LOWER(CONCAT('%', :keyword, '%')) OR LOWER(l.chiTiet) LIKE LOWER(CONCAT('%', :keyword, '%'))) AND " +
            "(:hanhDong IS NULL OR :hanhDong = '' OR :hanhDong = 'Tất cả' OR l.hanhDong = :hanhDong) AND " +
            "(:tuNgay IS NULL OR l.thoiGian >= :tuNgay) AND " +
            "(:denNgay IS NULL OR l.thoiGian <= :denNgay) " +
            "ORDER BY l.thoiGian DESC")
    List<LichSuHeThong> searchLogs(
            @Param("keyword") String keyword,
            @Param("hanhDong") String hanhDong,
            @Param("tuNgay") LocalDateTime tuNgay,
            @Param("denNgay") LocalDateTime denNgay
    );
}
