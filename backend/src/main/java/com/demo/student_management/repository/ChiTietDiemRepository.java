package com.demo.student_management.repository;

import com.demo.student_management.entity.ChiTietDiem;
import com.demo.student_management.entity.ChiTietDiemId;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface ChiTietDiemRepository extends JpaRepository<ChiTietDiem, ChiTietDiemId> {

    Optional<ChiTietDiem> findByHocSinh_IdHocSinhAndMonHoc_IdMonHocAndHocKy_IdHocKy(
            String idHocSinh,
            String idMonHoc,
            String idHocKy
    );

    List<ChiTietDiem> findByHocSinh_Lop_IdLopAndMonHoc_IdMonHocAndHocKy_IdHocKy(
            String idLop,
            String idMonHoc,
            String idHocKy
    );

    List<ChiTietDiem> findByHocSinh_Lop_IdLopAndHocKy_IdHocKy(
            String idLop,
            String idHocKy
    );

    List<ChiTietDiem> findByHocSinh_IdHocSinhAndHocKy_IdHocKy(
            String idHocSinh,
            String idHocKy
    );

    boolean existsByMonHoc_IdMonHoc(String idMonHoc);

    boolean existsByHocSinh_IdHocSinh(String idHocSinh);

    void deleteByHocSinh_IdHocSinh(String idHocSinh);
}