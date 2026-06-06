package com.demo.student_management.repository;

import com.demo.student_management.entity.PhanCongGiangDay;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface PhanCongGiangDayRepository extends JpaRepository<PhanCongGiangDay, String> {

    List<PhanCongGiangDay> findByGiaoVien_IdGiaoVien(String idGiaoVien);

    List<PhanCongGiangDay> findByGiaoVien_TaiKhoan_IdTaiKhoan(String idTaiKhoan);

    boolean existsByGiaoVien_TaiKhoan_IdTaiKhoanAndLop_IdLop(String idTaiKhoan, String idLop);

    boolean existsByGiaoVien_TaiKhoan_IdTaiKhoanAndLop_IdLopAndMonHoc_IdMonHoc(
            String idTaiKhoan, String idLop, String idMonHoc
    );

    boolean existsByGiaoVien_TaiKhoan_IdTaiKhoanAndLop_IdLopAndMonHoc_IdMonHocAndHocKy_IdHocKy(
            String idTaiKhoan, String idLop, String idMonHoc, String idHocKy
    );

    boolean existsByLop_IdLopAndMonHoc_IdMonHocAndHocKy_IdHocKy(
            String idLop, String idMonHoc, String idHocKy
    );

    boolean existsByMonHoc_IdMonHoc(String idMonHoc);
}
